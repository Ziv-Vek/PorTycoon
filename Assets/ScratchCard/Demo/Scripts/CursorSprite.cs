using System.Collections;
using UnityEngine;

namespace ScratchCardAsset.Demo
{
	public class CursorSprite : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer cursorSpriteRenderer;

		public SpriteRenderer SpriteRenderer => cursorSpriteRenderer;

		public void Show()
		{
			StopAllCoroutines();
			cursorSpriteRenderer.color = Color.white;
		}

		public void Hide()
		{
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(HideCursor());
			}
		}

		private IEnumerator HideCursor()
		{
			var alpha = cursorSpriteRenderer.color.a;
			while (cursorSpriteRenderer.color.a > 0)
			{
				alpha -= Time.deltaTime * 5f;
				cursorSpriteRenderer.color = new Color(1, 1, 1, alpha);
				yield return null;
			}
			cursorSpriteRenderer.color = Color.clear;
			gameObject.SetActive(false);
		}
	}
}