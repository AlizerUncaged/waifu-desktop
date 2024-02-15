using NAudio.Utils;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Text;
using Waifu.Utilities;

namespace NAudio.Wave
{
    public class WaveWriter : Stream // rewrite of wavefilewriter without the need of using a file
    {
        private MemoryStream? outStream;
        private BinaryWriter writer;
        private long dataSizePos;
        private long factSampleCountPos;
        private long dataChunkSize;
        private readonly WaveFormat format;

        public WaveWriter(WaveFormat format)
        {
            outStream = new MemoryStream();
            this.format = format;
            writer = new BinaryWriter(outStream, Encoding.UTF8);
            writer.Write("RIFF".ToBytes());
            writer.Write(0);
            writer.Write("WAVE".ToBytes());
            writer.Write("fmt ".ToBytes());
            format.Serialize(this.writer);
            CreateFactChunk();
            WriteDataChunkHeader();
        }

        private void WriteDataChunkHeader()
        {
            writer.Write("data".ToBytes());
            dataSizePos = outStream.Position;
            writer.Write(0);
        }

        private void CreateFactChunk()
        {
            if (HasFactChunk())
            {
                writer.Write("fact".ToBytes());
                writer.Write(4);
                factSampleCountPos = outStream.Position;
                writer.Write(0);
            }
        }

        private bool HasFactChunk()
        {
            return this.format.Encoding != WaveFormatEncoding.Pcm && this.format.BitsPerSample != 0;
        }

        public long DataChunkSize => this.dataChunkSize;

        public TimeSpan TotalTime =>
            TimeSpan.FromSeconds((double)Length / WaveFormat.AverageBytesPerSecond);


        public WaveFormat WaveFormat => this.format;

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override bool CanSeek => false;

        public override long Length => outStream.Length;

        public override long Position
        {
            get => this.outStream.Position;
            set => throw new InvalidOperationException("Repositioning a WaveFileWriter is not supported");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] data, int offset, int count)
        {
            if (outStream.Length + count > UInt32.MaxValue)
                throw new ArgumentException("WAV file too large", nameof(count));

            outStream.Write(data, offset, count);
            dataChunkSize += count;
        }

        public byte[] GetDataAsByteArray()
        {
            // Ensure all data is flushed to the stream
            Flush();

            // Store current position
            long currentPosition = outStream.Position;

            // Seek to the beginning of the stream
            outStream.Seek(0, SeekOrigin.Begin);

            // Read the entire stream into a byte array
            byte[] data = new byte[outStream.Length];
            outStream.Read(data, 0, data.Length);

            // Restore the original position
            outStream.Seek(currentPosition, SeekOrigin.Begin);

            return data;
        }

        private readonly byte[] value24 = new byte[3];

        public void WriteSample(float sample)
        {
            if (WaveFormat.BitsPerSample == 16)
            {
                writer.Write((Int16)(Int16.MaxValue * sample));
                dataChunkSize += 2;
            }
            else if (WaveFormat.BitsPerSample == 24)
            {
                var value = BitConverter.GetBytes((Int32)(Int32.MaxValue * sample));
                value24[0] = value[1];
                value24[1] = value[2];
                value24[2] = value[3];
                writer.Write(value24);
                dataChunkSize += 3;
            }
            else if (WaveFormat.BitsPerSample == 32 && WaveFormat.Encoding == WaveFormatEncoding.Extensible)
            {
                writer.Write(UInt16.MaxValue * (Int32)sample);
                dataChunkSize += 4;
            }
            else if (WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                writer.Write(sample);
                dataChunkSize += 4;
            }
            else
            {
                throw new InvalidOperationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
            }
        }

        public override void Flush()
        {
            var pos = writer.BaseStream.Position;
            UpdateHeader(writer);
            writer.BaseStream.Position = pos;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void DisposeA(bool disposing)
        {
            if (!disposing)
                return;

            if (this.outStream == null)
                return;

            try
            {
                this.UpdateHeader(this.writer);
            }
            finally
            {
                this.outStream.Dispose();
                this.outStream = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisposeA(disposing);
        }

        private void UpdateHeader(BinaryWriter writer)
        {
            writer.Flush();
            UpdateRiffChunk(writer);
            UpdateFactChunk(writer);
            UpdateDataChunk(writer);
        }

        public void UpdateHeader()
        {
            UpdateHeader(writer);
        }

        private void UpdateDataChunk(BinaryWriter writer)
        {
            writer.Seek((int)dataSizePos, SeekOrigin.Begin);
            writer.Write((UInt32)dataChunkSize);
        }

        private void UpdateRiffChunk(BinaryWriter writer)
        {
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write((UInt32)(outStream.Length - 8));
        }

        private void UpdateFactChunk(BinaryWriter writer)
        {
            if (HasFactChunk())
            {
                int bitsPerSample = (format.BitsPerSample * format.Channels);
                if (bitsPerSample != 0)
                {
                    writer.Seek((int)factSampleCountPos, SeekOrigin.Begin);

                    writer.Write((int)((dataChunkSize * 8) / bitsPerSample));
                }
            }
        }
    }
}