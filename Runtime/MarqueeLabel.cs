using UnityEngine.UIElements;
using UnityEngine;

[UxmlElement]
public partial class MarqueeLabel : Label
{
    private float _scrollSpeed = 10f;
    private float _pauseTime = 1f;

    [UxmlAttribute("scroll-speed")]
    public float ScrollSpeed
    {
        get => _scrollSpeed;
        set => _scrollSpeed = value;
    }

    [UxmlAttribute("pause-time")]
    public float PauseTime
    {
        get => _pauseTime;
        set => _pauseTime = value;
    }

    public MarqueeLabel() =>
        RegisterCallback<GeometryChangedEvent>(OnFirstGeometryChanged);

    private void OnFirstGeometryChanged(GeometryChangedEvent e)
    {
        if (_marqueeContainer == null && parent != null)
        {
            RuntimeInitialization();
            // Unregister to ensure this only runs once
            UnregisterCallback<GeometryChangedEvent>(OnFirstGeometryChanged);
        }
    }

    private VisualElement _marqueeContainer;
    private void RuntimeInitialization()
    {
        schedule.Execute(() =>
        {
            if (parent == null)
            {
                // Try again on the next frame
                RuntimeInitialization();
                return;
            }

            _marqueeContainer = new VisualElement();
            _marqueeContainer.style.position = resolvedStyle.position;
            if (resolvedStyle.position == Position.Absolute)
            {
                _marqueeContainer.style.left = resolvedStyle.left;
                _marqueeContainer.style.top = resolvedStyle.top;
                _marqueeContainer.style.right = resolvedStyle.right;
                _marqueeContainer.style.bottom = resolvedStyle.bottom;
            }
            _marqueeContainer.style.width = resolvedStyle.width;
            _marqueeContainer.style.height = resolvedStyle.height;
            _marqueeContainer.style.flexDirection = FlexDirection.Row;
            _marqueeContainer.style.overflow = Overflow.Hidden;

            this.style.position = Position.Absolute;
            this.style.width = StyleKeyword.Auto;

            int index = parent.IndexOf(this);
            parent.Insert(index, _marqueeContainer);
            parent.Remove(this);

            _marqueeContainer.Add(this);

            _marqueeContainer.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            schedule.Execute(Update).Every(16);
        });
    }

    private void OnGeometryChanged(GeometryChangedEvent e)
    {
        float textWidth = this.resolvedStyle.width;
        float containerWidth = _marqueeContainer.resolvedStyle.width;

        if (textWidth > containerWidth)
        {
            _timer = _pauseTime;
            _scrolling = true;
            _atStartPosition = true;
            _endPosition = containerWidth - textWidth;
            this.style.left = 0;
        }
        else
        {
            _scrolling = false;
            this.style.left = 0;
        }
    }

    private float _timer;
    private bool _scrolling;
    private bool _atStartPosition = true;
    private float _endPosition;
    private float _currentLeft;
    private void Update()
    {
        if (!_scrolling)
            return;

        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            return;
        }

        if (_atStartPosition)
        {
            _currentLeft -= _scrollSpeed * this.resolvedStyle.fontSize * Time.deltaTime;
            if (_currentLeft <= _endPosition)
            {
                _currentLeft = _endPosition;
                _timer = _pauseTime;
                _atStartPosition = false;
            }
        }
        else
        {
            _currentLeft = 0;
            _timer = _pauseTime;
            _atStartPosition = true;
        }

        this.style.left = _currentLeft;
    }
}