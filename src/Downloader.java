import java.io.*;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

public class Downloader {
	private static final String filePath = new File("").getAbsolutePath();

	public static void download(String link) throws Exception {
		System.out.println("読み込み中: " + link);
		long start = System.currentTimeMillis();

		final File saveFile = new File(filePath + "/testdata/input.txt");
		if (!saveFile.exists()) {
			saveFile.createNewFile();
		}

		URL url = new URL(link);
		HttpURLConnection con = (HttpURLConnection) url.openConnection();
		con.setDoInput(true);
		con.setConnectTimeout(10000);
		con.setReadTimeout(30000);
		con.setRequestMethod("GET");

		String line;
		BufferedReader br = new BufferedReader(new InputStreamReader(con.getInputStream()));
		BufferedWriter bw = new BufferedWriter(new FileWriter(saveFile));
		while ((line = br.readLine()) != null) {
			bw.write(line, 0, line.length());
			bw.write("\n", 0, 1);
		}
		br.close();
		bw.close();
		System.out.println("総: " + ((System.currentTimeMillis() - start) / 1000.0) + "(秒)");
	}

	public static void main(String... args) {
		try {
			Console console = System.console();
			download(console.readLine());
			console.flush();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
}

//javac Downloader.java; java Downloader
