#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class JSABSceneBuilder
{
    const string ScenePath = "Assets/Scenes/JSAB.unity";

    [MenuItem("Lucid/Prototipo JSAB/Crear escena")]
    public static void Build()
    {
        EnsureSpriteImporters();

        Sprite playerSprite = LoadSprite("Assets/personaje.png");
        Sprite squareSprite = LoadSprite("Assets/Sprites/pink_square.png");
        Sprite triangleSprite = LoadSprite("Assets/Sprites/pink_triangle.png");
        Sprite bulletSprite = LoadSprite("Assets/Sprites/pink_bullet.png");
        Sprite beamSprite = LoadSprite("Assets/Sprites/pink_beam.png");
        Sprite ringSprite = LoadSprite("Assets/Sprites/pink_ring.png");
        Sprite whiteSprite = LoadSprite("Assets/Sprites/white_square.png");

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject camGo = new GameObject("Main Camera");
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        Camera cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.4f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Hex("#0E0E1F");
        camGo.tag = "MainCamera";
        camGo.AddComponent<AudioListener>();

        GameObject beatGo = new GameObject("Beat Music");
        AudioSource audio = beatGo.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = true;
        audio.spatialBlend = 0f;
        BeatMusic beatMusic = beatGo.AddComponent<BeatMusic>();

        GameObject backdrop = new GameObject("Backdrop");
        SpriteRenderer bsr = backdrop.AddComponent<SpriteRenderer>();
        bsr.sprite = whiteSprite;
        bsr.color = Hex("#131328");
        bsr.sortingOrder = -10;
        Vector2 backdropSize = whiteSprite.bounds.size;
        float backdropHeight = cam.orthographicSize * 2f + 2f;
        float backdropWidth = cam.orthographicSize * 2f * cam.aspect + 2f;
        backdrop.transform.localScale = new Vector3(backdropWidth / backdropSize.x, backdropHeight / backdropSize.y, 1f);
        BeatPulse pulse = backdrop.AddComponent<BeatPulse>();
        pulse.beatMusic = beatMusic;

        GameObject hazards = new GameObject("Hazards");

        GameObject factoryGo = new GameObject("Hazard Factory");
        HazardFactory factory = factoryGo.AddComponent<HazardFactory>();
        factory.squareSprite = squareSprite;
        factory.triangleSprite = triangleSprite;
        factory.bulletSprite = bulletSprite;
        factory.beamSprite = beamSprite;
        factory.ringSprite = ringSprite;
        factory.container = hazards.transform;

        GameObject playerGo = new GameObject("Player");
        playerGo.transform.position = new Vector3(0f, -2f, 0f);
        SpriteRenderer psr = playerGo.AddComponent<SpriteRenderer>();
        psr.sprite = playerSprite;
        psr.sortingOrder = 10;
        CircleCollider2D pcol = playerGo.AddComponent<CircleCollider2D>();
        pcol.isTrigger = true;
        pcol.radius = 0.45f;
        Rigidbody2D prb = playerGo.AddComponent<Rigidbody2D>();
        prb.bodyType = RigidbodyType2D.Kinematic;
        PlayerController player = playerGo.AddComponent<PlayerController>();
        factory.player = player;

        GameObject spawnerGo = new GameObject("Hazard Spawner");
        HazardSpawner spawner = spawnerGo.AddComponent<HazardSpawner>();
        spawner.beatMusic = beatMusic;
        spawner.factory = factory;
        spawner.player = player;
        spawner.introBeats = 2;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        HUDController hud = canvasGo.AddComponent<HUDController>();

        Transform root = canvasGo.transform;
        Image[] dots = new Image[3];
        for (int i = 0; i < 3; i++)
            dots[i] = MakeImage(root, "Hp" + i, new Vector2(0f, 1f), new Vector2(56f + i * 40f, -52f), new Vector2(30f, 30f), Hex("#21C7F8"), whiteSprite);

        Image progressBg = MakeImage(root, "Progress", new Vector2(0.5f, 1f), new Vector2(0f, -44f), new Vector2(640f, 14f), Hex("#24244A"), whiteSprite);

        GameObject fillGo = new GameObject("Fill", typeof(Image));
        fillGo.transform.SetParent(progressBg.transform, false);
        RectTransform fr = (RectTransform)fillGo.transform;
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        fr.sizeDelta = Vector2.zero;
        Image fill = fillGo.GetComponent<Image>();
        fill.sprite = whiteSprite;
        fill.color = Hex("#21C7F8");
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 0f;
        fill.raycastTarget = false;

        GameObject markGo = new GameObject("CheckpointMark", typeof(Image));
        markGo.transform.SetParent(progressBg.transform, false);
        RectTransform mr = (RectTransform)markGo.transform;
        mr.anchorMin = mr.anchorMax = new Vector2(0.5f, 0.5f);
        mr.anchoredPosition = Vector2.zero;
        mr.sizeDelta = new Vector2(6f, 24f);
        Image mark = markGo.GetComponent<Image>();
        mark.sprite = whiteSprite;
        mark.color = Color.white;
        mark.raycastTarget = false;
        mark.enabled = false;

        Text centerText = MakeText(root, "CenterText", new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(1000f, 140f), 68, font, TextAnchor.MiddleCenter, Color.white, "");
        Text subText = MakeText(root, "SubText", new Vector2(0.5f, 0.5f), new Vector2(0f, -52f), new Vector2(900f, 44f), 26, font, TextAnchor.MiddleCenter, Hex("#A0A4B8"), "");
        MakeText(root, "Hint", new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(900f, 40f), 24, font, TextAnchor.MiddleCenter, Hex("#6A6E82"), "WASD · ESQUIVÁ EL ROSA");

        hud.hpDots = dots;
        hud.progressFill = fill;
        hud.checkpointMark = mark;
        hud.centerText = centerText;
        hud.subText = subText;

        GameObject managerGo = new GameObject("Game Manager");
        GameManager manager = managerGo.AddComponent<GameManager>();
        manager.player = player;
        manager.beatMusic = beatMusic;
        manager.spawner = spawner;
        manager.factory = factory;
        manager.hud = hud;

        EditorSceneManager.SaveScene(scene, ScenePath);

        List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        list.RemoveAll(s => s.path == ScenePath);
        list.Add(new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = list.ToArray();

        Selection.activeGameObject = playerGo;
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            sceneView.in2DMode = true;
            sceneView.pivot = Vector3.zero;
            sceneView.size = 12f;
            sceneView.Repaint();
        }
        Debug.Log("[JSAB] Escena creada en " + ScenePath + " — apretá Play.");
    }

    static void EnsureSpriteImporters()
    {
        AssetDatabase.Refresh();
        List<string> paths = new List<string> { "Assets/personaje.png" };
        if (Directory.Exists("Assets/Sprites"))
            paths.AddRange(Directory.GetFiles("Assets/Sprites", "*.png"));
        if (Directory.Exists("Assets/Sprites/player_frames"))
            paths.AddRange(Directory.GetFiles("Assets/Sprites/player_frames", "*.png"));
        foreach (string path in paths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(path);
                importer = AssetImporter.GetAtPath(path) as TextureImporter;
            }
            if (importer == null)
            {
                Debug.LogWarning("[JSAB] No pude importar " + path);
                continue;
            }
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }
        }
        AssetDatabase.SaveAssets();
    }

    static Sprite LoadSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            Debug.LogWarning("[JSAB] Sprite faltante: " + path);
        return sprite;
    }

    static Image MakeImage(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, Color color, Sprite sprite)
    {
        GameObject go = new GameObject(name, typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform r = (RectTransform)go.transform;
        r.anchorMin = r.anchorMax = anchor;
        r.anchoredPosition = position;
        r.sizeDelta = size;
        Image img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    static Text MakeText(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, int fontSize, Font font, TextAnchor align, Color color, string text)
    {
        GameObject go = new GameObject(name, typeof(Text));
        go.transform.SetParent(parent, false);
        RectTransform r = (RectTransform)go.transform;
        r.anchorMin = r.anchorMax = anchor;
        r.anchoredPosition = position;
        r.sizeDelta = size;
        Text t = go.GetComponent<Text>();
        t.font = font;
        t.fontSize = fontSize;
        t.alignment = align;
        t.color = color;
        t.text = text;
        t.raycastTarget = false;
        return t;
    }

    static Color Hex(string html)
    {
        return ColorUtility.TryParseHtmlString(html, out Color c) ? c : Color.white;
    }
}
#endif
