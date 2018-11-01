# EDIFabric-Implementation-Test

https://www.edifabric.com/

Simple use of the EDIFabric class. 

The final product (if it ever goes to production ) will probably not look anything like this.

This is just a proof of concept.

### Arguments 

[[-flag] [value] [-flag] [value] ... ... ]

* -d [void] debug
* -c [string] The class ( EDI Template type ) to use
* -p [string] The "Ship To" company
* -f [string] The name and extension of the file: blah.edi
* -s [string] The status Send, Receive, Respond

### Sub Classing

The program attempts to normalize incoming EDI data from the EDIFabric object created from the EDI file. In some case inevidably there will be caveats for each company. The sub class builds on the custom template feature to add extra fields as required by customer specs. Adding a template and sub-class is very easy and I include a fake ACME company template and sub-class.

### Templates

The class and subclass use the EDI Fabric templates that are just loose interfaces ( loose in that none of the variables or methods are required by the class ).
The sub template inherits the parent making it easy to impliment the sub-class with a simple reinstantiation of the template using the sub-class. It will auomatically inherit the parent. 

### History

I was using a Python EDI library and it works fine to create an object with no schema whatsoever but, looping that data would be like reinventing the wheel.

This library is much easier and worth the price for a profesional implementation.

![alt text](https://github.com/061375/EDIFabric-Implementation-Test/blob/master/EDI.Fabric/Files/Page1.png "Flow 1")

![alt text](https://github.com/061375/EDIFabric-Implementation-Test/blob/master/EDI.Fabric/Files/Page2.png "Flow 2")


