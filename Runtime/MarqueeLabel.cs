using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class MarqueeLabel : VisualElement
{
    private Label _textLabel;
    private float _scrollSpeed = 50f;
    private float _pauseTime = 1f;
    private float _timer;
    private bool _scrolling;
    private float _startPosition;
    private float _endPosition;
    private bool _atStart = true;

    [UxmlAttribute("text")]
    public string Text
    {
        get => _textLabel?.text;
        set => SetText(value);
    }

    [UxmlAttribute("scroll-speed")]
    public float ScrollSpeed
    {
        get => _scrollSpeed;
        set => SetScrollSpeed(value);
    }

    [UxmlAttribute("pause-time")]
    public float PauseTime
    {
        get => _pauseTime;
        set => SetPauseTime(value);
    }

    public MarqueeLabel()
    {
        style.overflow = Overflow.Hidden;

        _textLabel = new Label();
        _textLabel.style.position = Position.Absolute;
        _textLabel.style.left = 0;
        hierarchy.Add(_textLabel);

        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        schedule.Execute(Update).Every(16);
    }

    public void Setup(string text, float scrollSpeed, float pauseTime)
    {
        SetText(text);
        _scrollSpeed = scrollSpeed;
        _pauseTime = pauseTime;
    }

    public void SetText(string text) =>
        _textLabel.text = text;

    public void SetScrollSpeed(float speed) =>
        _scrollSpeed = speed;

    public void SetPauseTime(float time) =>
        _pauseTime = time;

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        float textWidth = _textLabel.resolvedStyle.width;
        float containerWidth = resolvedStyle.width;

        if (textWidth > containerWidth)
        {
            _scrolling = true;
            _startPosition = 0f;
            _endPosition = containerWidth - textWidth;
            _textLabel.style.left = _startPosition;
            _timer = _pauseTime;
            _atStart = true;
        }
        else
        {
            _scrolling = false;
            _textLabel.style.left = 0;
        }
    }

    private void Update()
    {
        if (!_scrolling)
            return;

        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        float currentLeft = _textLabel.style.left.value.value;

        if (_atStart)
        {
            currentLeft -= _scrollSpeed * Time.deltaTime;
            if (currentLeft <= _endPosition)
            {
                currentLeft = _endPosition;
                _timer = _pauseTime;
                _atStart = false;
            }
            _textLabel.style.left = currentLeft;
        }
        else
        {
            _timer = _pauseTime;
            _textLabel.style.left = _startPosition;
            _atStart = true;
        }
    }
}