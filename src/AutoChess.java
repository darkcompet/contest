import javax.imageio.ImageIO;
import javax.swing.*;
import java.awt.*;
import java.awt.event.MouseEvent;
import java.awt.event.MouseMotionAdapter;
import java.awt.image.BufferedImage;
import java.io.File;

public class AutoChess {
   // Rectangle which user select by dragging on the screen image
   private Rectangle selection;

   public AutoChess() {
   }

   private void start() throws Exception {
      BufferedImage screenImage = captureScreen();
      startDragOnScreenImage(screenImage);
   }

   private void writeImage(BufferedImage image, File outFile) throws Exception {
      boolean status = ImageIO.write(image, "png", outFile);
      System.out.println("Screen Captured ? " + status + ", File Path: " + outFile.getPath());

      SwingUtilities.invokeLater(new Runnable() {
         public void run() {
         }
      });
   }

   private void startDragOnScreenImage(final BufferedImage screen) {
      final BufferedImage screenCopy = new BufferedImage(screen.getWidth(), screen.getHeight(), screen.getType());
      final JLabel screenLabel = new JLabel(new ImageIcon(screenCopy));
      JScrollPane screenScroll = new JScrollPane(screenLabel);

      screenScroll.setPreferredSize(new Dimension(screen.getWidth() * 4 / 5, screen.getHeight() * 4 / 5));

      JPanel panel = new JPanel(new BorderLayout());
      panel.add(screenScroll, BorderLayout.CENTER);

      final JLabel selectionLabel = new JLabel("Drag a rectangle in the screen shot!");
      panel.add(selectionLabel, BorderLayout.SOUTH);

      repaint(screen, screenCopy);
      screenLabel.repaint();

      screenLabel.addMouseMotionListener(new MouseMotionAdapter() {
         Point start = new Point();

         @Override
         public void mouseMoved(MouseEvent me) {
            start = me.getPoint();
            repaint(screen, screenCopy);
            selectionLabel.setText("Start Point: " + start);
            screenLabel.repaint();
         }

         @Override
         public void mouseDragged(MouseEvent me) {
            Point end = me.getPoint();
            selection = new Rectangle(start, new Dimension(end.x - start.x, end.y - start.y));
            repaint(screen, screenCopy);
            screenLabel.repaint();
            selectionLabel.setText("Selection: " + selection);
         }
      });

      JOptionPane.showMessageDialog(null, panel);
   }

   /**
    * @param orig original image
    * @param copy copy of original image
    */
   private void repaint(BufferedImage orig, BufferedImage copy) {
      Graphics2D g = copy.createGraphics();
      g.drawImage(orig, 0, 0, null);

      if (selection != null) {
         g.setColor(Color.RED);
         g.draw(selection);
         g.setColor(new Color(255, 255, 255, 150));
         g.fill(selection);
      }
      g.dispose();
   }

   private BufferedImage captureScreen() throws Exception {
      Robot robot = new Robot();
      Dimension screenSize = Toolkit.getDefaultToolkit().getScreenSize();
      Rectangle screenBounds = new Rectangle(screenSize);
      BufferedImage screenImage = robot.createScreenCapture(screenBounds);
      return screenImage;
   }

   public static void main(String[] args) {
      try {
         Thread.sleep(3000);
         new AutoChess().start();
      }
      catch (Exception e) {
         e.printStackTrace();
      }
   }
}