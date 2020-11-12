using Microsoft.Xna.Framework;

namespace PigeonEngine.component {
	public interface Poolable {
		void OnRecycle();
		void OnReuse();
	}
}