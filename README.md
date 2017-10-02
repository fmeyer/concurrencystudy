# Concurrency study in C#

# Instructions

* Install mono or dotnet core
* Check out this project
* Compile and execute the Server project 
* Compile and execute the Client project to see the simple test case execution; 

More detailed test cases are implemented into the IntegrationTests 


# Design 

Both Client and server use simple threading to enable concurrency, Instead of Thread Start/Join, I used the callback pattern available into the socket API. I've also, wrapped the async result into a Task<T> interface to enable the use of the async method. 

I believe my lack of experience with the .net platform made my design non-standard or messy to the expert eyes, which isn't something I can remedy in such short term.

# Problems

Socket programming without implementing a simple sequence protocol is quite difficult since messages can and probably will
arrive out of order since it depends on external networking factors. Implementing a message protocol like: 

    <seq> <operation> <op1> <op2> 

Would be enough to allow a more robust concurrency implementation and avoid race conditions as I've been facing when two threads are waiting for a message, and I notify using the MRE causing both to rush into the queue. 

I could implement a retry mechanism, but this won't guarantee the order of the message.
