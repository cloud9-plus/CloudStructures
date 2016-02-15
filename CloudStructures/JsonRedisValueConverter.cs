﻿using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CloudStructures
{
    public class JsonRedisValueConverter : ObjectRedisValueConverterBase
    {
        protected override object DeserializeCore(Type type, byte[] value)
        {
            using (var ms = new MemoryStream(value))
            using (var sr = new StreamReader(ms, Encoding.UTF8))
            {
                var result = new JsonSerializer().Deserialize(sr, type);
                return result;
            }
        }

        protected override byte[] SerializeCore(object value, out long resultSize)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Encoding.UTF8))
                {
                    new JsonSerializer().Serialize(sw, value);
                }
                var result = ms.ToArray();
                resultSize = result.Length;
                return result;
            }
        }
    }

    public class GZipJsonRedisValueConverter : ObjectRedisValueConverterBase
    {
        readonly System.IO.Compression.CompressionLevel compressionLevel;

        public GZipJsonRedisValueConverter()
            : this(System.IO.Compression.CompressionLevel.Fastest)
        {

        }

        public GZipJsonRedisValueConverter(System.IO.Compression.CompressionLevel compressionLevel)
        {
            this.compressionLevel = compressionLevel;
        }

        protected override object DeserializeCore(Type type, byte[] value)
        {
            using (var ms = new MemoryStream(value))
            using (var gzip = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress))
            using (var sr = new StreamReader(gzip, Encoding.UTF8))
            {
                var result = new JsonSerializer().Deserialize(sr, type);
                return result;
            }
        }

        protected override byte[] SerializeCore(object value, out long resultSize)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new System.IO.Compression.GZipStream(ms, compressionLevel))
                using (var sw = new StreamWriter(gzip))
                {
                    new JsonSerializer().Serialize(sw, value);
                }
                var result = ms.ToArray();
                resultSize = result.Length;
                return result;
            }
        }
    }
}