#Kiln
A simple command line utility that bakes all referenced image files in a HTML document into embedded Data URI's, thus making the HTML file standalone.

##Why?
Sometimes you want a standalone HTML file that doesn't have external references. For example, I use [Markdown](http://daringfireball.net/projects/markdown/) to write technical documentation. It's convenient to publish that markdown to HTML and then email or otherwise share the HTML file to colleagues without bothering to publish it onto a server. Since my documentation includes images, it's a hassle to have to .zip up a directory of files - I just want to send a single file that works on its own! This is a much nicer experience for the recipient, too; he simply double clicks the file to view it without having to decompress a jumble of files and subdirectories.

It's already easy enough to bake CSS directly into a HTML file. This utility makes it just as easy to embed the images for all of its <img> tags, too.

##But, but...
###Why wouldn't I just publish my content to PDF and share that?
You can do that, but HTML is more interactive. Your content can include javascript and interactivity. For example, my documentation includes nicely formatted scrollable panes for source code.

But if PDF gets the job done for you, great! No need for Kiln.

###Why wouldn't I just publish to my server and share a link?
If that's convenient for you, then go ahead! You don't need or want Kiln for that.

##Platforms
Tested on Mac OS X under Mono and on Windows with the CLR. Probably works fine in Linux, too.
