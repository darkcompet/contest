package tool;

import javax.imageio.ImageIO;
import javax.swing.*;
import java.awt.*;
import java.awt.image.*;
import java.io.File;
import java.io.IOException;
import java.util.Random;

import static java.lang.Math.*;

public final class ImageUtil {
	public int color(int a, int r, int g, int b) {
		return ((a & 0xff) << 24) | ((r & 0xff) << 16) | ((g & 0xff) << 8) | (b & 0xff);
	}

	public int a(int color) {
		return (color >> 24) & 0xff;
	}

	public int r(int color) {
		return (color >> 16) & 0xff;
	}

	public int g(int color) {
		return (color >> 8) & 0xff;
	}

	public int b(int color) {
		return (color & 0xff);
	}

	public int luminance(int rgb) {
		int r = (rgb >> 16) & 0xff, g = (rgb >> 8) & 0xff, b = rgb & 0xff;
		return (int) Math.round(0.299 * r + 0.587 * g + 0.114 * b);
	}

	public boolean isCompatible(int color1, int color2) {
		return abs(luminance(color1) - luminance(color2)) >= 128;
	}

	public BufferedImage loadImage(File file) {
		try {
			return ImageIO.read(file);
		} catch (IOException e) {
			e.printStackTrace();
		}
		return null;
	}

	public void save(BufferedImage bitmap, String formatName, String filePath) {
		try {
			File file = new File(filePath);
			if (!file.exists()) file.createNewFile();
			ImageIO.write(bitmap, formatName, file);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public void show(String title, BufferedImage bitmap) {
		JFrame frame = new JFrame(title);
		JPanel panel = new JPanel() {
			@Override
			protected void paintComponent(Graphics g) {
				super.paintComponent(g);
				Graphics2D gg = (Graphics2D) g;
				gg.drawImage(bitmap, 0, 0, null);
			}
		};
		panel.setPreferredSize(new Dimension(bitmap.getWidth(), bitmap.getHeight()));
		JScrollPane scrollPane = new JScrollPane(panel, 20, 30);
		frame.add(scrollPane);
		frame.setResizable(true);
		frame.setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
		frame.setVisible(true);
		frame.setLocationRelativeTo(null);
		frame.pack();
	}

	public BufferedImage scale(BufferedImage input, int dstWidth, int dstHeight) {
		int width = input.getWidth(), height = input.getHeight();
		BufferedImage output = new BufferedImage(dstWidth, dstHeight, BufferedImage.TYPE_INT_ARGB);
		for (int i = 0; i < dstWidth; ++i) {
			for (int j = 0; j < dstHeight; ++j) {
				int rgb = input.getRGB(i * width / dstWidth, j * height / dstHeight);
				output.setRGB(i, j, rgb);
			}
		}
		return output;
	}

	public BufferedImage grayScale(BufferedImage input) {
		final int w = input.getWidth(), h = input.getHeight();
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int j = 0; j < h; ++j) {
			for (int i = 0; i < w; ++i) {
				int rgb = input.getRGB(i, j);
				int r = (rgb >> 16) & 0xff;
				int g = (rgb >> 8) & 0xff;
				int b = (rgb) & 0xff;
				int gray = (int) Math.round(0.299 * r + 0.587 * g + 0.114 * b);
				Color color = new Color(gray, gray, gray);
				output.setRGB(i, j, color.getRGB());
			}
		}
		return output;
	}

	public BufferedImage detectEdge(BufferedImage input) {
		final int w = input.getWidth(), h = input.getHeight();
		int[][] filter1 = {{-1, 0, 1}, {-2, 0, 2}, {-1, 0, 1}};
		int[][] filter2 = {{1, 2, 1}, {0, 0, 0}, {-1, -2, -1}};
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int y = 1; y < h - 1; y++) {
			for (int x = 1; x < w - 1; x++) {
				// getPrefixSum 3-by-3 array of colors in neighborhood
				int[][] gray = new int[3][3];
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 3; j++) {
						int rgb = input.getRGB(x - 1 + i, y - 1 + j);
						int r = (rgb >> 16) & 0xff;
						int g = (rgb >> 8) & 0xff;
						int b = (rgb) & 0xff;
						gray[i][j] = (int) Math.round(0.299 * r + 0.587 * g + 0.114 * b);
					}
				}

				// apply filter
				int gx = 0, gy = 0;
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 3; j++) {
						gx += gray[i][j] * filter1[i][j];
						gy += gray[i][j] * filter2[i][j];
					}
				}
				int magnitude = (int) Math.sqrt(gx * gx + gy * gy);
				magnitude = (magnitude < 0) ? 0 : (magnitude > 255) ? 255 : magnitude;
				magnitude = 255 - magnitude;
				Color grayscale = new Color(magnitude, magnitude, magnitude);
				output.setRGB(x, y, grayscale.getRGB());
			}
		}
		return output;
	}

	public BufferedImage zoom(BufferedImage input, int cx, int cy, double zoomTime) {
		final int w = input.getWidth(), h = input.getHeight();
		int black = Color.BLACK.getRGB();
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int ty = 0; ty < h; ++ty) {
			for (int tx = 0; tx < w; ++tx) {
				int sx = (int) (cx + (tx - cx) / zoomTime);
				int sy = (int) (cy + (ty - cy) / zoomTime);
				if (sx >= 0 && sx < w && sy >= 0 && sy < h) output.setRGB(tx, ty, input.getRGB(sx, sy));
				else output.setRGB(tx, ty, black);
			}
		}
		return output;
	}

	public BufferedImage glass(BufferedImage input) {
		final int w = input.getWidth(), h = input.getHeight();
		Random random = new Random();
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				int tx = (w + x + 1 + random.nextInt(10) - 5) % w;
				int ty = (h + y + 1 + random.nextInt(10) - 5) % h;
				output.setRGB(tx, ty, input.getRGB(x, y));
			}
		}
		return output;
	}

	public BufferedImage wave(BufferedImage input) {
		final int w = input.getWidth(), h = input.getHeight();
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				int tx = x;
				int ty = (int) (y + 20 * sin(PI * x / 32));
				if (ty >= 0 && ty < h) output.setRGB(tx, ty, input.getRGB(x, y));
			}
		}
		return output;
	}

	public BufferedImage swirl(BufferedImage input, double cx, double cy, boolean clockwise) {
		final int w = input.getWidth(), h = input.getHeight();
		if (cx < 0 || cx >= w || cy < 0 || cy >= h) throw new RuntimeException();
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				double a = x - cx, b = y - cy;

				double angle = PI * hypot(a, b) / 256;
				if (clockwise) angle = -angle;
				final double cos = cos(angle), sin = sin(angle);

				int xx = (int) (cx + b * sin + a * cos);
				int yy = (int) (cy + b * cos - a * sin);
				if (xx >= 0 && xx < w && yy >= 0 && yy < h) output.setRGB(xx, yy, input.getRGB(x, y));
			}
		}
		return output;
	}

	public BufferedImage fade(BufferedImage input1, BufferedImage input2, double ratio) {
		int w = input1.getWidth(), h = input1.getHeight();
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				int rgb1 = input1.getRGB(x, y), rgb2 = input2.getRGB(x, y);
				int r1 = (rgb1 >> 16) & 0xff, g1 = (rgb1 >> 8) & 0xff, b1 = (rgb1) & 0xff;
				int r2 = (rgb2 >> 16) & 0xff, g2 = (rgb2 >> 8) & 0xff, b2 = (rgb2) & 0xff;
				int r = (int) (ratio * r1 + (1 - ratio) * r2);
				int g = (int) (ratio * g1 + (1 - ratio) * g2);
				int b = (int) (ratio * b1 + (1 - ratio) * b2);
				int rgb = new Color(r, g, b).getRGB();
				output.setRGB(x, y, rgb);
			}
		}
		return output;
	}

	public BufferedImage rotate(BufferedImage input, double cx, double cy, double angle, boolean clockwise) {
		final int w = input.getWidth(), h = input.getHeight();
		if (cx < 0 || cx >= w || cy < 0 || cy >= h) throw new RuntimeException();
		angle = toRadians(angle);
		if (clockwise) angle = -angle;
		final double cosA = cos(angle), sinA = sin(angle);
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		for (int y = 0; y < h; ++y) {
			for (int x = 0; x < w; ++x) {
				double a = x - cx, b = y - cy;
				int xx = (int) (cx + b * sinA + a * cosA);
				int yy = (int) (cy + b * cosA - a * sinA);
				if (xx >= 0 && xx < w && yy >= 0 && yy < h) output.setRGB(xx, yy, input.getRGB(x, y));
			}
		}
		return output;
	}

	public BufferedImage flip(BufferedImage input, boolean vertical) {
		final int w = input.getWidth(), h = input.getHeight();
		BufferedImage output = new BufferedImage(w, h, BufferedImage.TYPE_INT_ARGB);
		final int lx = vertical ? (w >> 1) : w - 1;
		final int ly = vertical ? h - 1 : (h >> 1);
		for (int j = 0; j <= ly; ++j) {
			for (int i = 0; i <= lx; ++i) {
				int xx = vertical ? w - 1 - i : i;
				int yy = vertical ? j : h - 1 - j;
				output.setRGB(i, j, input.getRGB(xx, yy));
				output.setRGB(xx, yy, input.getRGB(i, j));
			}
		}
		return output;
	}

	public Image removeBackground(BufferedImage input, final Color color) {
		final ImageFilter filter = new RGBImageFilter() {
			final int opaque_bkg_color = color.getRGB() | 0xff000000;

			@Override
			public int filterRGB(final int x, final int y, final int rgb) {
				if ((rgb | 0xff000000) == opaque_bkg_color) {
					return 0x00ffffff & rgb;
				} else {
					return rgb;
				}
			}
		};
		final ImageProducer ip = new FilteredImageSource(input.getSource(), filter);
		return Toolkit.getDefaultToolkit().createImage(ip);
	}

	public void test() {
		BufferedImage image = loadImage(new File("/Users/VuanCoal/mine/cloud/picture/favorite/mewtwo.png"));
		show("MewTwo", image);
	}
}
