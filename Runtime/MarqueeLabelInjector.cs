using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MarqueeLabelInjector : MonoBehaviour
{
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        var documents = FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
        foreach (var document in documents)
            document.gameObject.AddComponent<MarqueeLabelInjector>();
    }

    public void Start()
    {
        var document = GetComponent<UIDocument>();
        if (document != null && document.rootVisualElement != null)
            TransformLabels(document.rootVisualElement);
    }

    private static void TransformLabels(VisualElement root)
    {
        foreach (var child in root.Children().ToList())
        {
            if (child is Label label && child is not MarqueeLabel)
                ReplaceWithMarqueeLabel(label);

            TransformLabels(child);
        }
    }

    private static void ReplaceWithMarqueeLabel(Label original)
    {
        var parent = original.parent;
        if (parent == null)
            return;

        var marquee = new MarqueeLabel()
        {
            text = original.text,
            name = original.name,
            ScrollSpeed = 10f,
            PauseTime = 1f
        };
        
        foreach (var styleClass in original.GetClasses())
            marquee.AddToClassList(styleClass);

        int index = parent.IndexOf(original);
        parent.Remove(original);
        parent.Insert(index, marquee);

        marquee.RegisterCallback<GeometryChangedEvent>(e =>
        {
            marquee.style.fontSize = original.resolvedStyle.fontSize;
            marquee.style.width = original.resolvedStyle.width;
            marquee.style.height = original.resolvedStyle.height;
        });
    }
}