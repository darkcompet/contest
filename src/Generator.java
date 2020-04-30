import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Random;

public class Generator {
	private final String lineSeparator = System.lineSeparator();
	private final String rootPath = new File("").getAbsolutePath();
	private BufferedReader in;
	private BufferedWriter out;
	private Random random = new Random();

	private void start() throws Exception {
		in = new BufferedReader(new FileReader(rootPath + "/testdata/in.txt"));
		out = new BufferedWriter(new FileWriter(rootPath + "/testdata/out.txt"));
		generate();
		in.close();
		out.close();
	}

	public static void main(String... args) {
		try {
			new Generator().start();
		}
		catch (Exception e) {
			e.printStackTrace();
		}
	}

	private <T> void print(Object obj) throws IOException {
		String s = obj + "";
		out.write(s, 0, s.length());
	}

	private void print(String format, Object... args) throws IOException {
		if (args != null) {
			format = String.format(format, args);
		}
		out.write(format, 0, format.length());
	}

	private void println(Object obj) throws IOException {
		String s = obj + lineSeparator;
		out.write(s, 0, s.length());
	}

	private void println(String format, Object... args) throws IOException {
		if (args != null) {
			format = String.format(format, args);
		}
		format += lineSeparator;
		out.write(format, 0, format.length());
	}

	private void generate() throws Exception {
		StringBuilder sb = new StringBuilder();
		String line;
		
		while ((line = in.readLine()) != null) {
			for (String w : line.split(",")) {
				w = w.trim().replace("\"", "").trim();
				println("String " + w + " = \"" + w + "\";");
			}
		}
		
		out.write(sb.toString());
	}
}
// javac Generator.java; java Generator
