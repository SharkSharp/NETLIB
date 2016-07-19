# NETLIB
A c# lib to abstract and create a friendly interface for network jobs.

Design Goals: This library is designed to be...

* Fast enough for games
* Robust enough for enterprise applications
* Easy enough for quick learning

# A brief introduction to NETLIB

### BasePack
It is the basic unit of the network communication, in other words all the information that travels over 
the network is converted in BasePack before transmission and is subsequently reassembled by the receiver.
It simplifies operations with the network buffer and handle reading and writing.

### Publisher
Describes a pack publisher, which will be responsible for managing the incoming packs, adding them in a
queue and by setting an event signal to the Consumer that there is a pack in the queue.

### Consumer
Describes the class that will be responsible for consuming the packages, meaning it will build 
packages with the buffers published by a publisher and will launch an event for every pack to be treated.

### Protocol
Responsible for managing a communication protocol, in other words, analyze an incoming
packet, check for a method of treatment registered for that type of package,
if any, the method is called to handle the package, if not a generic event
is called to handle the incoming pack. Idealised to facilitate handling packages and management
protocols, especially in cases where the client continuously migrates between different protocols.

### IOPackHandler
Better manage the incoming and outgoing a pack using a Protocol to redistribute the packs.
It has an internal dictionary of Protocols that can be exchanged for the currently used.
