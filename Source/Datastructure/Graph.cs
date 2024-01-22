namespace Compet.Datastructure;

public class Graph {
	/// <summary>
	/// Vertice count.
	/// </summary>
	private readonly int N;

	/// <summary>
	/// Edge count.
	/// </summary>
	private int M;

	private List<int>[] adj;

	public Graph(int N) {
		var adj = this.adj = new List<int>[N];
		for (var index = 0; index < N; ++index) {
			adj[index] = new();
		}

		var dist = this.dist = new int[N][];
		const int INF = 1 << 29;
		for (var index = 0; index < N; ++index) {
			dist[index] = new int[N];
			Array.Fill(dist[index], INF);
		}
	}

	public void AddEdge(int u, int v, int w = 1) {
		this.adj[u].Add(v);
		this.adj[v].Add(u);
		++this.M;
	}

	public int VertexCount => this.N;
	public int EdgeCount => this.M;

	private int[][] dist;
	public void PreprocessDistanceCalculation() {
		var N = this.N;
		var dist = this.dist;
		for (var k = 0; k < N; ++k) {
			for (var u = 0; u < N; ++u) {
				for (var v = 0; v < N; ++v) {
					dist[u][v] = Math.Min(dist[u][v], dist[u][k] + dist[k][v]);
				}
			}
		}
	}
	public int DistanceOf(int u, int v) {
		return this.dist[u][v];
	}
}
