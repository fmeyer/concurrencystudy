# Concurrency study in C#

# Instructions

* Install mono or dotnet core
* Checkout this project
* Compile and execute the Server project 
* Compile and execute the Client project to see the simple test case execution; 

More detailed testcases are implemented into the IntegrationTests 


# Design 

Both Client and server uses simple threading to enable concurrency, Instead of Thread Start/Join I used the 
callback pattern available into the socket API, which enables the 

I've wrapped the async result into a Task<T> interface in order to enable the use of the async method. 

I believe my lack of experience with the .net platform made my design non-standard or messy to the expert eyes, which isn't someting I can remedy in such short term.

# Problems

Socket programming without implementing a simple sequence protocol is quite dificult, since messages can and probably will
arrive out of order since it depends on external networking factors. Implementing a simple message like 

    <seq> <operation> <op1> <op2> 

Would be enough to implement more robust concurrency and avoid race conditions as I've been facing when two threads are waiting for a message and I notify using the MRE causing both to rush into the queue. 

I cold implement a retry mechanism but this won't garantee the order of the message.
