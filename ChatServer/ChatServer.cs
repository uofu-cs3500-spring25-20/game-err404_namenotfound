// <copyright file="ChatServer.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

using CS3500.Networking;
using System.Collections.Concurrent;
using System.Text;

namespace CS3500.Chatting;

/// <summary>
///   A simple ChatServer that handles clients separately and replies with a static message.
/// </summary>
public partial class ChatServer
{

    private static ConcurrentDictionary<NetworkConnection, string> clients = new ConcurrentDictionary<NetworkConnection, string>();

    /// <summary>
    ///   The main program.
    /// </summary>
    /// <param name="args"> ignored. </param>
    /// <returns> A Task. Not really used. </returns>
    private static void Main( string[] args )
    {
        Server.StartServer( HandleConnect, 11_000 );
        Console.Read(); // don't stop the program.
    }


    /// <summary>
    ///   <pre>
    ///     When a new connection is established, enter a loop that receives from and
    ///     replies to a client.
    ///   </pre>
    /// </summary>
    ///
    private static void HandleConnect( NetworkConnection connection )
    {
        // handle all messages until disconnect.
        try
        {
            string ClientName = connection.ReadLine();
            clients.TryAdd(connection, ClientName);
            connection.Send("Welcome to the chat server, " + ClientName + "!");

            while ( true )
            {
                var message = connection.ReadLine( );

                BroadcastMessage(message, connection);
            }
        }
        catch ( Exception )
        {
            // do anything necessary to handle a disconnected client in here
            clients.TryRemove(connection, out _);
        }
    }

    /// <summary>
    /// Broadcasts the message to each of the clients currently connected to the server.
    /// </summary>
    /// <param name="message">Message you want to send</param>
    /// <param name="connection">Sending connection</param>
    public static void BroadcastMessage(string message, NetworkConnection connection)
    {

        var clientConnections = clients.Keys.ToList();

        foreach (var ClientConnection in clientConnections)
        {
            if (clients.TryGetValue(connection, out string? clientName))
            {
                ClientConnection.Send(clientName + " : " + message);
            }
        }
    }
}