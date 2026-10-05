using System;
using System.Reflection;
using System.Xml.Linq;
using Engine;

namespace Game;

public sealed class UIExpansionWidget : CanvasWidget
{
    private readonly ComponentPlayer m_player;
    private readonly LabelWidget m_time;
    private readonly LabelWidget m_position;
    private readonly CanvasWidget m_panel;
    private readonly CanvasWidget m_debugPanel;

    private BevelledButtonWidget? m_debugButton;
    private bool m_buttonInstalled;
    private bool m_debugVisible;

    public UIExpansionWidget(ComponentPlayer player)
    {
        m_player = player;

        LoadContents(
            this,
            ContentManager.Get<XElement>("Widgets/UIExpansionWidget")
        );

        m_panel = Children.Find<CanvasWidget>("Panel");
        m_time = Children.Find<LabelWidget>("Time");
        m_position = Children.Find<LabelWidget>("Position");
        m_debugPanel = Children.Find<CanvasWidget>("DebugPanel");
    }

    public override void Update()
    {
        base.Update();

        ModSettingsManager.TryGet(
            out bool hud,
            UIExpansionModLoader.PackageName,
            "UIExpansionSettings",
            "HUD"
        );

        ModSettingsManager.TryGet(
            out bool worldInfo,
            UIExpansionModLoader.PackageName,
            "UIExpansionSettings",
            "WorldInfo"
        );

        m_panel.IsVisible = hud;

        InstallDebugButton();
        UpdateHud(worldInfo);
        UpdateDebugHud();
    }

    private void UpdateHud(bool worldInfo)
    {
        if (!m_panel.IsVisible)
            return;

        m_time.Text = $"Time: {DateTime.Now:HH:mm:ss}";

        if (!worldInfo)
        {
            m_position.IsVisible = false;
            return;
        }

        ComponentBody? body = m_player.Entity.FindComponent<ComponentBody>(true);
        if (body != null)
        {
            Vector3 p = body.Position;
            m_position.Text = $"X: {p.X:0}  Y: {p.Y:0}  Z: {p.Z:0}";
        }
        else
        {
            m_position.Text = "X: --  Y: --  Z: --";
        }

        m_position.IsVisible = true;
    }

    private void InstallDebugButton()
    {
        if (m_buttonInstalled)
            return;

        StackPanelWidget? moreContents = m_player.GuiWidget?.Children.Find<StackPanelWidget>("MoreContents");
        BitmapButtonWidget? helpButton = moreContents?.Children.Find<BitmapButtonWidget>("HelpButton");

        if (moreContents == null || helpButton == null)
            return;

        var button = new BevelledButtonWidget
        {
            Name = "UIExpansion.DebugButton",
            Text = "Debug",
            Size = new Vector2(92f, 64f),
            Margin = new Vector2(4f, 0f)
        };

        if (!InsertAfter(moreContents, helpButton, button))
        {
            Log.Warning("UI Expansion: could not insert Debug button after HelpButton.");
            return;
        }

        m_debugButton = button;
        m_buttonInstalled = true;
    }

    private static bool InsertAfter(StackPanelWidget parent, Widget after, Widget child)
    {
        object children = parent.Children;
        Type type = children.GetType();

        MethodInfo? indexOf = type.GetMethod(
            "IndexOf",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(Widget) },
            modifiers: null);

        MethodInfo? insert = type.GetMethod(
            "Insert",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: new[] { typeof(int), typeof(Widget) },
            modifiers: null);

        if (indexOf == null || insert == null)
            return false;

        int index = (int)indexOf.Invoke(children, new object[] { after })!;
        if (index < 0)
            return false;

        insert.Invoke(children, new object[] { index + 1, child });
        return true;
    }

    private void UpdateDebugHud()
    {
        if (m_debugButton != null && m_debugButton.IsClicked)
            m_debugVisible = !m_debugVisible;

        m_debugPanel.IsVisible = m_debugVisible;
        if (!m_debugVisible)
            return;

        ComponentBody? body = m_player.Entity.FindComponent<ComponentBody>(true);
        if (body == null)
        {
            Children.Find<LabelWidget>("DebugPosition").Text = "XYZ: -- -- --";
            return;
        }

        Vector3 p = body.Position;
        Children.Find<LabelWidget>("DebugPosition").Text =
            $"XYZ: {p.X:0.00} / {p.Y:0.00} / {p.Z:0.00}";
    }
}
