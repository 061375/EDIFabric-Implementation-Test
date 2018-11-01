# EDIFabric-Implementation-Test

https://www.edifabric.com/

Simple use of the EDIFabric class. I am not a C# programmer as of yet (15+ years OOP in other languages) but, I needed it for this task and I am finding it to be a great langauge.
NOTE: I did take a C# course on Lynda.com about a year ago, so it's not like I just started coding in it out of the blue.

The final product (if it ever goes to production ) will not look anything like this.

This is just a proof of concept.

### Arguments

* -d debug
* -c The class ( EDI Template type ) to use
* -p The "Ship To" company
* -f The name and extension of the file: blah.edi
* -s The status Send, Receive, Respond

### Sub Classing

The program attempts to normalize incoming EDI data from the EDIFabric object created from the EDI file. In some case inevidably there will be caveats for each company. The sub class builds on the custom template feature to add extra fields as required by customer specs. Adding a template and sub-class is very easy and I include a fake ACME company template and sub-class.

### History

I was using a Python EDI library and it works fine to create an object with no schema whatsoever but, looping that data would be like reinventing the wheel.

This library is much easier and worth the price for a profesional implementation.




