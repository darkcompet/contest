# Contest

For Online Programming Contest.
This project is written mainly in C# and should be run with Mono.


## Quick start

- Run

	```bash
	cd Source; mcs Contest.cs; mono Contest.exe;
	```


## How this project was made

	```bash
	# Make console app for code-support by C# extension
	dotnet new console -o contest

	# Init git
	git init

	# Add submodules
	mkdir -p Tool/Compet; cd Tool/Compet;
	git submodule add https://github.com/darkcompet/csharp-core.git
	```


## Resources

	```bash
	# Atcoder test data url
	https://www.dropbox.com/sh/nx3tnilzqz7df8a/AAAYlTq2tiEHl5hsESw6-yfLa?dl=0

	# Ref
	C++: https://atcoder.jp/contests/abc310/submissions/43606683
	C#: https://atcoder.jp/contests/abc310/submissions/43585432
	```
