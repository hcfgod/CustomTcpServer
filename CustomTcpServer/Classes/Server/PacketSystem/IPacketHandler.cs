﻿namespace InfinityServer.Classes.Server.PacketSystem
{
    public interface IPacketHandler
    {
        void Handle(Packet packet, ClientHandler clientHandler);
    }
}
