# NETLIB
A c# lib to abstract and create a friendly interface for network jobs.

## Why use NETLIB?
NETLIB encapsulates TCPClient in a friendlier layer to the developer, manages the entry of new clients, redirecting them to its management method. Manages incoming and outgoing packs, segmented them by types and redirecting exactly where they will be treated. It also provides an abstraction for communication protocols, facilitating the work of software using various protocols.

Design Goals: This library is designed to be...

* Fast enough for games
* Robust enough for enterprise applications
* Easy enough for quick learning

# A brief introduction to NETLIB

### BasePack
It is the basic unit of the network communication, in other words all the information that travels over 
the network is converted in BasePack before transmission and is subsequently reassembled by the receiver.
It simplifies operations with the network buffer and handle reading and writing.

#### BasePack example

```cs
void BasePackUseExample()
{
            int i = 5;
            char c = 'a';
            string str = "test";
            float f = 0.5F;
            double d = 5.3F;
            bool b = false;

            int ib;
            char cb;
            string strb;
            float fb;
            double db;
            bool bb;

            BasePack newPack = new BasePack();
            newPack.ID = 10;

            newPack.PutInt(i);
            newPack.PutChar(c);
            newPack.PutString(str);
            newPack.PutFloat(f);
            newPack.PutDouble(d);
            newPack.PutBool(b);

            //Do something

            ib = newPack.GetInt();
            cb = newPack.GetChar();
            strb = newPack.GetString();
            fb = newPack.GetFloat();
            db = newPack.GetDouble();
            bb = newPack.GetBool();
}
```

### Publisher
Describes a pack publisher, which will be responsible for managing the incoming packs, adding them in a
queue and by setting an event signal to the Consumer that there is a pack in the queue.

### Consumer
Describes the class that will be responsible for consuming the packs, meaning it will build 
packs with the buffers published by a publisher and will launch an event for every pack to be treated.

### Protocol
Responsible for managing a communication protocol, in other words, analyze an incoming
pack, check for a method of treatment registered for that type of pack,
if any, the method is called to handle the pack, if not a generic event
is called to handle the incoming pack. Idealised to facilitate handling packs and management
protocols, especially in cases where the client continuously migrates between different protocols.

#### Protocol Example

```cs
public void CreateProtocolExampleMethod()
{
   var newProtocol = new Protocol<BasePack>("newProtocol");
   newProtocol[0] += ZeroIDHandler;
   newProtocol.ReceivedPack += DefaultIDHundler;
}

private static void ZeroIDHandler(Consumer<BasePack> consumer, BasePack receivedPack)
{
   //Do something with the packs that have ID = 0.
}

private static void DefaultIDHundler(Consumer<BasePack> consumer, BasePack receivedPack)
{
   //Do something with the packs that do not have a handler method registered.
}
```

### IOPackHandler
Better manage the incoming and outgoing a pack using a Protocol to redistribute the packs.
It has an internal dictionary of Protocols that can be exchanged for the currently used.

#### IOPackHandler example
```cs
public void CreateIOPackHandlerExampleMethod()
{
   var newProtocol = new Protocol<BasePack>("newProtocol");
   newProtocol[0] += ZeroIDHandler;
   newProtocol.ReceivedPack += DefaultIDHundler;

   client = new IOBasePackHandler(new TCPPublisher("127.0.0.1", 1975), newProtocol);
   client.Start();
}

private static void ZeroIDHandler(Consumer<BasePack> consumer, BasePack receivedPack)
{
   //Do something with the packs that have ID = 0.
}

private static void DefaultIDHundler(Consumer<BasePack> consumer, BasePack receivedPack)
{
   //Do something with the packs that do not have a handler method registered.
}
```

# Chat code example using NETLIB

### Chat server example

```cs
using NETLIB;
using NETLIB.TCP;
using System;

namespace ChatExempleClient
{
    class Program
    {
        static IOBasePackHandler client;
        static Protocol<BasePack> chatProtocol;
        static string name;

        static void Main(string[] args)
        {
            chatProtocol = new Protocol<BasePack>("chatProtocol");
            chatProtocol[0] += MessagePackHandle;

            client = new IOBasePackHandler(new TCPPublisher("127.0.0.1", 1975), chatProtocol);
            client.Start();

            Console.WriteLine("Your name please:");
            name = Console.ReadLine();

            string aux = Console.ReadLine();
            while (aux != "exit")
            {
                var pack = new BasePack();
                pack.ID = 0;
                pack.PutString(name + ": " + aux);
                client.SendPack(pack);
                aux = Console.ReadLine();
            }

            client.CloseConnection();
        }

        private static void MessagePackHandle(Consumer<BasePack> consumer, BasePack receivedPack)
        {
            Console.WriteLine(receivedPack.GetString());
        }
    }
}
```

### Chat client example

```cs
using NETLIB;
using NETLIB.TCP.Server;
using System.Collections.Generic;

namespace ChatExempleServer
{
    class Program
    {
        static TCPListenerHandler listenerHandler;
        static List<IOBasePackHandler> clients;
        static Protocol<BasePack> chatProtocol;

        static void Main(string[] args)
        {
            clients = new List<IOBasePackHandler>();

            chatProtocol = new Protocol<BasePack>("chatProtocol");
            chatProtocol[0] += MessagePackHandle;

            listenerHandler = new TCPListenerHandler();
            listenerHandler.ReceivedConnection += ListenerHandlerReceivedConnection;
            listenerHandler.BeginListen(1975);
        }

        private static void MessagePackHandle(Consumer<BasePack> consumer, BasePack receivedPack)
        {
            foreach (var client in clients)
            {
                if (client != consumer)
                {
                    client.SendPack(receivedPack);
                }
            }
        }

        private static void ListenerHandlerReceivedConnection(Publisher publisher)
        {
            var newClient = new IOBasePackHandler(publisher, chatProtocol);
            clients.Add(newClient);
            newClient.Start();
        }
    }
}

```
