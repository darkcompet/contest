package tool;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import javax.swing.*;
import java.awt.*;
import java.awt.event.InputEvent;
import java.awt.image.BufferedImage;
import java.io.*;
import java.net.URL;
import java.util.*;
import java.util.List;

public final class Util {
	public static final String USER_HOME_DIR = System.getProperty("user.home");
	public static final String LINE_SEPARATOR = System.getProperty("line.separator");
	public static final String FILE_SEPARATOR = File.separator;
	public static final Locale LOCALE_JP = new Locale("ja", "JP");
	public static final Locale LOCALE_US = new Locale("en", "US");
	public static final Locale LOCALE_VN = new Locale("vi", "VN");

	private static final Gson GSON = new GsonBuilder().setDateFormat("yyyy/MM/dd HH:mm:ss")
			.excludeFieldsWithoutExposeAnnotation().setPrettyPrinting()
			.create();

	private Util() {}

	public static boolean createNewFile(String filePath) {
		return createNewFile(new File(filePath));
	}

	public static boolean createNewFile(File file) {
		File parent = file;
		List<File> list = new ArrayList<>();
		while (!parent.exists()) {
			list.add(parent);
			parent = parent.getParentFile();
		}
		for (int i = list.size() - 1; i >= 0; --i) {
			File now = list.get(i);
			try {
				if (i > 0) now.mkdirs();
				else now.createNewFile();
			} catch (Exception e) {
				System.err.println("Failed to create " + now.getPath());
				e.printStackTrace();
				return false;
			}
		}
		return true;
	}

	public static void writeToFile(File file, byte[] data) throws IOException {
		if (file == null || data == null) return;
		if (!file.exists()) file.createNewFile();
		OutputStream os = new FileOutputStream(file);
		os.write(data, 0, data.length);
		os.close();
	}

	public static void writeToFile(String filePath, byte[] data) throws IOException {
		writeToFile(new File(filePath), data);
	}

	public static void writeToFile(String filePath, String data) throws IOException {
		writeToFile(new File(filePath), data);
	}

	public static void writeToFile(File file, String data) throws IOException {
		if (file == null || data == null) return;
		if (!createNewFile(file)) return;
		BufferedWriter w = new BufferedWriter(new FileWriter(file));
		w.write(data, 0, data.length());
		w.close();
	}

	public static <T> void saveCache(T obj, String filePath) throws IOException {
		if (!createNewFile(filePath)) return;
		writeToFile(filePath, convertToJson(obj));
	}

	public static <T> T loadCache(String filePath, Class<T> classOfT) {
		return convertToGson(streamToString(filePath), classOfT);
	}

	public static String streamToString(String filePath) {
		if (!createNewFile(filePath)) return null;
		String line;
		StringBuilder sb = new StringBuilder();
		try {
			BufferedReader br = new BufferedReader(new FileReader(filePath));
			while ((line = br.readLine()) != null) sb.append(line).append(LINE_SEPARATOR);
			br.close();
		} catch (Exception e) {
			return null;
		}
		return sb.toString();
	}

	public boolean isNull(String s) {
		return s == null || s.length() == 0;
	}

	public static <T> boolean isEquals(T a, T b) {
		if (a == null) return b == null;
		return a.equals(b);
	}

	public boolean isWhite(String s) {
		return s == null || s.trim().length() == 0;
	}

	public static <K, V> Map<K, V> sortMap(Map<K, V> map, Comparator<Map.Entry<K, V>> comparator) {
		Map.Entry<K, V> a[] = new Map.Entry[map.size()];
		map.entrySet().toArray(a);
		Arrays.sort(a, comparator);
		Map<K, V> sortedMap = new LinkedHashMap<>();
		for (Map.Entry<K, V> elem : a) sortedMap.put(elem.getKey(), elem.getValue());
		return sortedMap;
	}

	public static void assertEquals(boolean statement) {
		if (!statement) throw new RuntimeException();
	}

	public ResourceBundle getBundle(String baseName, Locale locale) {
		ResourceBundle bundle = null;
		try {
			bundle = ResourceBundle.getBundle(baseName, locale);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return bundle;
	}

	public URL getResourceUrl(String key, ResourceBundle bundle) {
		if (bundle == null) throw new RuntimeException("bundle is null");
		String value = bundle.getString(key);
		return getClass().getClassLoader().getResource(value);
	}

	public static void setAntialias(Graphics2D gg) {
		gg.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
		gg.setRenderingHint(RenderingHints.KEY_TEXT_ANTIALIASING, RenderingHints.VALUE_TEXT_ANTIALIAS_ON);
	}

	public static <T> T convertToGson(String json, Class<T> classOfT) {
		return GSON.fromJson(json, classOfT);
	}

	public static String convertToJson(Object obj) {
		return GSON.toJson(obj);
	}

	public static void beep() {
		Toolkit.getDefaultToolkit().beep();
	}

	public static Process executeCommand(String command) throws IOException {
		return Runtime.getRuntime().exec(command);
	}

	public static GraphicsDevice[] getScreenDevices() {
		return GraphicsEnvironment.getLocalGraphicsEnvironment().getScreenDevices();
	}

	public static void setCursor(JFrame frame, BufferedImage buf_img) {
		Cursor cursor = Toolkit.getDefaultToolkit().createCustomCursor(buf_img, new Point(0, 0), "cursor");
		frame.setCursor(cursor);
	}

	public static Point getCurrentMouseLocation() {
		return MouseInfo.getPointerInfo().getLocation();
	}

	public static void moveMouseTo(int x, int y) {
		for (GraphicsDevice gd : getScreenDevices()) {
			for (GraphicsConfiguration config : gd.getConfigurations()) {
				Rectangle bound = config.getBounds();
				if (bound.contains(x, y)) {
					try {
						int cx = x - bound.x, cy = y - bound.y;
						Robot robot = new Robot(gd);
						robot.mouseMove(cx, cy);
					} catch (AWTException e) {
						e.printStackTrace();
					}
					return;
				}
			}
		}
	}

	public static BufferedImage getScreenImage() throws AWTException {
		Rectangle screenRect = new Rectangle(Toolkit.getDefaultToolkit().getScreenSize());
		return new Robot().createScreenCapture(screenRect);
	}

	public static void clickMouse(Robot robot) {
		robot.mousePress(InputEvent.BUTTON1_MASK);
		robot.mouseRelease(InputEvent.BUTTON1_MASK);
	}

	public static void wheelMouse(Robot robot, int value) {
		robot.mouseWheel(value);
	}

	public static Dimension getScreenSize() {
		GraphicsDevice gd = GraphicsEnvironment.getLocalGraphicsEnvironment().getDefaultScreenDevice();
		int width = gd.getDisplayMode().getWidth();
		int height = gd.getDisplayMode().getHeight();
		return new Dimension(width, height);
		//		return Toolkit.getDefaultToolkit().getScreenSize();
	}

	public static GraphicsDevice getDefaultScreenDevices() {
		GraphicsEnvironment ge = GraphicsEnvironment.getLocalGraphicsEnvironment();
		return ge.getDefaultScreenDevice();
	}

	public static void setFullScreen(DisplayMode dm, JFrame frame) {
		GraphicsDevice gd = getDefaultScreenDevices();
		gd.setFullScreenWindow(frame);
		if (dm != null && gd.isDisplayChangeSupported()) {
			gd.setDisplayMode(dm);
		}
	}

	public static Object deepClone(Object obj) throws IOException, ClassNotFoundException {
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ObjectOutputStream oos = new ObjectOutputStream(os);
		oos.writeObject(obj);
		oos.close();

		ObjectInputStream is = new ObjectInputStream(new ByteArrayInputStream(os.toByteArray()));
		Object res = is.readObject();
		is.close();
		return res;
	}

	public void test() throws Exception {}
}
