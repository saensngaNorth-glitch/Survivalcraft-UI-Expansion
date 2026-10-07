using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using Engine;

namespace Game;

public sealed class UIExpansionWidget : CanvasWidget
{
    private readonly ComponentPlayer m_player;
    private readonly CanvasWidget m_debugPanel;

    private BitmapButtonWidget? m_debugButton;
    private bool m_buttonInstalled;
    private bool m_debugVisible;

    private readonly Stopwatch m_fpsTimer = Stopwatch.StartNew();
    private int m_frameCount;
    private int m_fps;

    public UIExpansionWidget(ComponentPlayer player)
    {
        m_player = player;

        LoadContents(
            this,
            ContentManager.Get<XElement>("Widgets/UIExpansionWidget")
        );

        m_debugPanel =
            Children.Find<CanvasWidget>("DebugPanel");
    }

    public override void Update()
    {
        base.Update();

        InstallDebugButton();
        UpdateFps();
        UpdateDebugHud();
    }

    private void InstallDebugButton()
    {
        if (m_buttonInstalled)
            return;

        StackPanelWidget? moreContents =
            m_player.GuiWidget?
                .Children
                .Find<StackPanelWidget>("MoreContents");

        BitmapButtonWidget? helpButton =
            moreContents?
                .Children
                .Find<BitmapButtonWidget>("HelpButton");

        if (moreContents == null || helpButton == null)
            return;

        var button = new BitmapButtonWidget
        {
            Name = "UIExpansion.DebugButton",
            Size = new Vector2(68f, 64f),

            NormalSubtexture =
                ContentManager.Get<Subtexture>(
                    "Textures/Atlas/EditItemButton"),

            ClickedSubtexture =
                ContentManager.Get<Subtexture>(
                    "Textures/Atlas/EditItemButton_Pressed"),

            Margin = new Vector2(4f, 0f)
        };

        if (!InsertAfter(moreContents, helpButton, button))
        {
            Log.Warning(
                "UI Expansion: could not insert Debug button."
            );

            return;
        }

        m_debugButton = button;
        m_buttonInstalled = true;
    }

    private static bool InsertAfter(
        StackPanelWidget parent,
        Widget after,
        Widget child)
    {
        object children = parent.Children;
        Type type = children.GetType();

        MethodInfo? indexOf =
            type.GetMethod(
                "IndexOf",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(Widget) },
                modifiers: null);

        MethodInfo? insert =
            type.GetMethod(
                "Insert",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic,
                binder: null,
                types: new[] { typeof(int), typeof(Widget) },
                modifiers: null);

        if (indexOf == null || insert == null)
            return false;

        int index =
            (int)indexOf.Invoke(
                children,
                new object[] { after })!;

        if (index < 0)
            return false;

        insert.Invoke(
            children,
            new object[]
            {
                index + 1,
                child
            });

        return true;
    }

    private void UpdateFps()
    {
        m_frameCount++;

        if (m_fpsTimer.ElapsedMilliseconds < 1000)
            return;

        m_fps =
            (int)(
                m_frameCount /
                m_fpsTimer.Elapsed.TotalSeconds);

        m_frameCount = 0;
        m_fpsTimer.Restart();
    }

    private void UpdateDebugHud()
    {
        if (m_debugButton != null &&
            m_debugButton.IsClicked)
        {
            m_debugVisible = !m_debugVisible;
        }

        m_debugPanel.IsVisible = m_debugVisible;

        if (!m_debugVisible)
            return;

        LabelWidget fps =
            Children.Find<LabelWidget>("DebugFPS");

        LabelWidget position =
            Children.Find<LabelWidget>("DebugPosition");

        LabelWidget block =
            Children.Find<LabelWidget>("DebugBlock");

        LabelWidget time =
            Children.Find<LabelWidget>("DebugTime");

        LabelWidget world =
            Children.Find<LabelWidget>("DebugWorld");

        LabelWidget health =
            Children.Find<LabelWidget>("DebugHealth");

        LabelWidget stamina =
            Children.Find<LabelWidget>("DebugStamina");

        fps.Text =
            $"FPS: {m_fps}";

        ComponentBody? body =
            m_player.Entity.FindComponent<ComponentBody>(true);

        if (body != null)
        {
            Vector3 p = body.Position;

            position.Text =
                $"พิกัด: {p.X:0.00} / {p.Y:0.00} / {p.Z:0.00}";

            block.Text =
                $"บล็อกใต้เท้า: {Math.Floor(p.X):0} / {Math.Floor(p.Y - 1f):0} / {Math.Floor(p.Z):0}";
        }
        else
        {
            position.Text = "พิกัด: -- / -- / --";
            block.Text = "บล็อกใต้เท้า: -- / -- / --";
        }

        time.Text =
            $"เวลา: {GetGameTimeText()}";

        world.Text =
            $"โลก: {GetWorldText()}";

        health.Text =
            $"พลังชีวิต: {GetHealthText()}";

        stamina.Text =
            $"ความอึด: {GetStaminaText()}";
    }

    private string GetGameTimeText()
    {
        try
        {
            object? project =
                typeof(GameManager)
                    .GetProperty(
                        "Project",
                        BindingFlags.Static |
                        BindingFlags.Public |
                        BindingFlags.NonPublic)
                    ?.GetValue(null);

            if (project == null)
                return "--:--";

            MethodInfo? findSubsystem =
                project.GetType().GetMethod(
                    "FindSubsystem",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

            if (findSubsystem == null)
                return "--:--";

            MethodInfo generic =
                findSubsystem.MakeGenericMethod(
                    typeof(SubsystemTime));

            object? subsystem =
                generic.Invoke(
                    project,
                    null);

            if (subsystem == null)
                return "--:--";

            object? value =
                GetMemberValue(
                    subsystem,
                    "GameTime");

            if (value == null)
            {
                value =
                    GetMemberValue(
                        subsystem,
                        "Time");
            }

            if (value == null)
                return "--:--";

            double seconds =
                Convert.ToDouble(value);

            TimeSpan gameTime =
                TimeSpan.FromSeconds(seconds);

            return
                $"{gameTime.Hours:00}:{gameTime.Minutes:00}";
        }
        catch
        {
            return "--:--";
        }
    }

    private string GetWorldText()
    {
        try
        {
            object? project =
                typeof(GameManager)
                    .GetProperty(
                        "Project",
                        BindingFlags.Static |
                        BindingFlags.Public |
                        BindingFlags.NonPublic)
                    ?.GetValue(null);

            if (project == null)
                return "--";

            object? value =
                GetMemberValue(
                    project,
                    "WorldName");

            if (value != null)
                return value.ToString() ?? "--";

            value =
                GetMemberValue(
                    project,
                    "World");

            if (value != null)
            {
                object? name =
                    GetMemberValue(
                        value,
                        "Name");

                if (name != null)
                    return name.ToString() ?? "--";
            }
        }
        catch
        {
        }

        return "--";
    }

    private string GetHealthText()
    {
        try
        {
            ComponentHealth? component =
                m_player.Entity
                    .FindComponent<ComponentHealth>(true);

            if (component == null)
                return "-- / --";

            object? health =
                GetMemberValue(
                    component,
                    "Health");

            object? maxHealth =
                GetMemberValue(
                    component,
                    "MaxHealth");

            if (health != null && maxHealth != null)
            {
                return
                    $"{Convert.ToInt32(health)} / {Convert.ToInt32(maxHealth)}";
            }

            if (health != null)
            {
                return
                    $"{Convert.ToInt32(health)} / --";
            }
        }
        catch
        {
        }

        return "-- / --";
    }

    private string GetStaminaText()
    {
        try
        {
            object? stamina =
                GetMemberValue(
                    m_player,
                    "Stamina");

            object? maxStamina =
                GetMemberValue(
                    m_player,
                    "MaxStamina");

            if (stamina != null && maxStamina != null)
            {
                return
                    $"{Convert.ToInt32(stamina)} / {Convert.ToInt32(maxStamina)}";
            }

            if (stamina != null)
            {
                return
                    $"{Convert.ToInt32(stamina)} / --";
            }
        }
        catch
        {
        }

        return "-- / --";
    }

    private static object? GetMemberValue(
        object instance,
        string name)
    {
        Type type = instance.GetType();

        PropertyInfo? property =
            type.GetProperty(
                name,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (property != null)
            return property.GetValue(instance);

        FieldInfo? field =
            type.GetField(
                name,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

        if (field != null)
            return field.GetValue(instance);

        return null;
    }
}
