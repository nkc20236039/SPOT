using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimatedImage : MonoBehaviour
{
    [SerializeField, Header("Relative path from StreamingAssets folder")] private string filePath;
    [HideInInspector] public bool playGif;

    private Image _image;

    private readonly List<Sprite> _frames = new List<Sprite>();
    private readonly List<float> _frameDelay = new List<float>();

    private int _currentFrame = 0;
    private float _time = 0.0f;

    private void Start()
    {
        gameObject.GetComponent<Image>().enabled = true;
        if (string.IsNullOrWhiteSpace(filePath)) return;
        _image = GetComponent<Image>();

        var path = Path.Combine(Application.streamingAssetsPath, filePath);

        using (var decoder = new MG.GIF.Decoder(File.ReadAllBytes(path)))
        {
            var img = decoder.NextImage();

            while (img != null)
            {
                _frames.Add(Texture2DtoSprite(img.CreateTexture()));
                _frameDelay.Add(img.Delay / 1000.0f);
                img = decoder.NextImage();
            }
        }

        _image.sprite = _frames[0];
    }

    private void Update()
    {
        if (_frames.Count == 0 || !playGif) return;

        _time += Time.deltaTime;

        if (_time >= _frameDelay[_currentFrame])
        {
            _currentFrame = (_currentFrame + 1) % _frames.Count;
            _time = 0.0f;

            _image.sprite = _frames[_currentFrame];
            if (_currentFrame >= _frames.Count) { playGif = false; }
        }
    }

    private static Sprite Texture2DtoSprite(Texture2D tex)
        => Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
}