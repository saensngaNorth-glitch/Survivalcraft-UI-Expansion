using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Game;

public class UIExpansionWidget : CanvasWidget
{
    private ComponentPlayer m_player;
    private BitmapButtonWidget? m_debugButton;
    private bool m_buttonInstalled = false;
    private CanvasWidget m_debugPanel;
    private bool m_debugVisible = false;

    public UIExpansionWidget(ComponentPlayer player)
    {
        m_player = player;
        
        // โหลด XML ผ่าน ModsManager เพื่ออ่านไฟล์จากโฟลเดอร์มอดโดยตรง ไม่ให้เกมเด้ง
        XElement xelement;
        using (Stream stream = ModsManager.OpenFile("Widgets/UIExpansionWidget.xml"))
        {
            xelement = XElement.Load(stream);
        }
        
        // ส่ง XElement เข้า LoadContents ตามที่คอมไพเลอร์ต้องการ
        LoadContents(this, xelement);
        
        m_debugPanel = Children.Find<CanvasWidget>("DebugPanel");
        
        InstallDebugButton();
    }

    private void UpdateUpdate()
    {
        UpdateDebugHud();
    }

    private void InstallDebugButton()
    {
        if (m_buttonInstalled)
            return;

        StackPanelWidget? moreContents = m_player.GuiWidget?.Children.Find<StackPanelWidget>("MoreContents");
        BitmapButtonWidget? helpButton = moreContents?.Children.Find<BitmapButtonWidget>("HelpButton");

        if (moreContents == null || helpButton == null)
            return;

        var button = new BitmapButtonWidget
        {
            Name = "UIExpansion.DebugButton",
            Size = new Vector2(64f, 64f),
            NormalSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/EditItemButton"),
            ClickedSubtexture = ContentManager.Get<Subtexture>("Textures/Atlas/EditItemButton_Pressed"),
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

        object? indexObj = indexOf.Invoke(children, new object[] { after });
        if (indexObj == null)
            return false;

        int index = (int)indexObj;
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
            return;

        Vector3 p = body.Position;
        
        var debugPosLabel = Children.Find<LabelWidget>("DebugPosition");
        if (debugPosLabel != null)
        {
            debugPosLabel.Text = $"XYZ: {p.X:0.00} / {p.Y:0.00} / {p.Z:0.00}";
        }
    }
}
