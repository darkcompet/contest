# Contest

For Online Programming Contest.
This project is written mainly in C# and should be run with Mono.


## How this project was made

	```bash
	# Make console app for code-support by C# extension
	dotnet new console -o contest

	# Init git
	git init

	# Add submodules
	mkdir -p Tool/Compet; cd Tool/Compet;
	git submodule add https://github.com/darkcompet/csharp-core.git
	git submodule add https://github.com/darkcompet/csharp-net-core
	```
