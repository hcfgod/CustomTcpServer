﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InfinityServer.Classes.Server.PacketSystem
{
    public class Packet
    {
        // Metadata
        public Guid PacketID { get; set; }
        public string PacketType { get; set; }
        public long SequenceNumber { get; set; }
        public DateTime Timestamp { get; set; }

        // Payload
        public byte[] Data { get; set; }
        public byte[] Message { get; set; }

        // Security
        public string Checksum { get; set; }
        public bool EncryptionFlag { get; set; } // TODO: Set up and use this

        // Additional fields
        public string Version { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string SenderID { get; set; }
        public string ReceiverID { get; set; }
        public string FileExtestion { get; set; }

        public void GenerateChecksum()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(this.Data);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                this.Checksum = builder.ToString();
            }
        }
    }
}
