# NETLIB
A c# lib to abstract and create a friendly interface for network jobs

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
It is the class that should get the arriving packs and put in the pack queue, that will be used by Consumer,
and should implement a method that allows to send packs in the network.

### Consumer
Class that get a pack in the pack queue and throws a event in a parallel thread, sending yourself and the pack for someone to treat the pack.

### IOPackHandler
Derived from Consumer, it uses a hash table to analyze the incoming packet ID and submit to a specific system function, pre registered by the programmer, sending the package exactly where it should go. 
