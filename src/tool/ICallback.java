package tool;

public abstract class ICallback {
	public <T> void run(T... args) {}

	public <R, T> R runBack(T... args) {return null;}
}
