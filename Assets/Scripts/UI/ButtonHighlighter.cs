using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHighlighter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // Color naranja oscuro (#ff7514 en RGB)
    public Color darkOrange = new Color(1f, 0.459f, 0.078f); // #ff7514

    // Colores base
    public Color normalBgColor = new Color(0f, 0f, 0f, 0f); // Transparente
    public Color normalTextColor = new Color(1f, 1f, 1f, 0.85f); // Texto blanco claro
    public Color highlightTextColor = new Color(1f, 1f, 1f, 1f); // Texto blanco brillante

    // Componentes del botón
    private GameObject bgObject, glowOverlayObject, textObject;
    private Image bgImage, glowOverlayImage;
    private TextMeshProUGUI buttonText;
    private Outline outlineEffect;

    // Animación
    private Vector3 originalScale;
    private bool isHovered;
    public float scaleSpeed = 8f;
    public float fadeSpeed = 10f;

    // Textura para el degradado
    private Texture2D gradientTexture;
    private Material gradientMaterial;

    void Start()
    {
        // CreateGradientTexture();
        InitializeComponents();
        originalScale = transform.localScale;
    }

    private void CreateGradientTexture()
    {
        // Crear textura de 256x1 píxeles para el degradado horizontal
        gradientTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false);
        gradientTexture.wrapMode = TextureWrapMode.Clamp;
        gradientTexture.filterMode = FilterMode.Bilinear;

        // Crear degradado de derecha a izquierda: naranja oscuro -> negro
        for (int x = 0; x < 256; x++)
        {
            // Progresión de 1 a 0 de derecha a izquierda
            float progress = 1f - (x / 255f);

            // Color que va de naranja oscuro a negro
            Color gradientColor = Color.Lerp(Color.black, darkOrange, progress);
            gradientColor.a = progress * 0.8f; // Alpha también se desvanece

            for (int y = 0; y < 1; y++)
            {
                gradientTexture.SetPixel(x, y, gradientColor);
            }
        }
        gradientTexture.Apply();

        // Crear material con la textura de degradado
        gradientMaterial = new Material(Shader.Find("UI/Default"));
        gradientMaterial.mainTexture = gradientTexture;
    }

    private void InitializeComponents()
    {
        // Buscar componentes existentes
       /* bgObject = transform.Find("ButtonBG")?.gameObject;
        textObject = transform.Find("Text")?.gameObject;

        if (bgObject == null)
        {
            // CreateBackground();
        }
        else
        {
            bgImage = bgObject.GetComponent<Image>();
        }

        if (textObject == null)
        {
            textObject = transform.Find("ButtonText")?.gameObject;
        }

        if (textObject != null)
        {
            buttonText = textObject.GetComponent<TextMeshProUGUI>();
            outlineEffect = textObject.GetComponent<Outline>();
        }*/

        // CreateGlowOverlay();

    bgImage = GetComponentInChildren<Image>();

    buttonText = GetComponentInChildren<TextMeshProUGUI>();
    outlineEffect = buttonText != null ? buttonText.GetComponent<Outline>() : null;

    if (buttonText == null)
    {
        Debug.LogWarning($"[ButtonHighlighter] No se encontró TextMeshProUGUI en {gameObject.name}");
    }
}


    private void CreateBackground()
    {
        bgObject = new GameObject("ButtonBG", typeof(RectTransform));
        bgObject.transform.SetParent(transform, false);
        bgImage = bgObject.AddComponent<Image>();
        bgImage.color = normalBgColor;

        RectTransform bgRect = bgObject.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Mover al fondo
        bgObject.transform.SetAsFirstSibling();
    }

    private void CreateGlowOverlay()
    {
        glowOverlayObject = new GameObject("GlowOverlay", typeof(RectTransform));
        glowOverlayObject.transform.SetParent(transform, false);
        glowOverlayImage = glowOverlayObject.AddComponent<Image>();

        // Aplicar material con degradado
        glowOverlayImage.material = gradientMaterial;
        glowOverlayImage.type = Image.Type.Sliced;

        // Configurar para que ocupe todo el botón
        RectTransform overlayRect = glowOverlayObject.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        // Inicialmente transparente
        glowOverlayImage.color = new Color(1f, 1f, 1f, 0f);

        // Colocar sobre el fondo pero bajo el texto
        glowOverlayObject.transform.SetSiblingIndex(1);
    }

    void Update()
    {
        // if (buttonText == null) return;

        // Animación de escala suave
        /*Vector3 targetScale = isHovered ? originalScale * 1.05f : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        // Animación del degradado
        // float targetAlpha = isHovered ? 1f : 0f;
        // Color currentColor = glowOverlayImage.color;
        // currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
        // glowOverlayImage.color = currentColor;

        // Animación del texto
        if (isHovered && buttonText != null)
        {
            buttonText.color = Color.Lerp(buttonText.color, highlightTextColor, Time.deltaTime * fadeSpeed);
            if (outlineEffect != null)
            {
                outlineEffect.enabled = true;
                outlineEffect.effectColor = new Color(0f, 0f, 0f, 1f);
            }
        }
        else
        {
            buttonText.color = Color.Lerp(buttonText.color, normalTextColor, Time.deltaTime * fadeSpeed);
            if (outlineEffect != null)
            {
                outlineEffect.effectColor = new Color(0f, 0f, 0f, 0.8f);
            }
        }

        // Efecto de brillo adicional en el fondo base durante hover
        
        if (bgImage != null)
        {
            Color bgTarget = isHovered ?
                new Color(normalBgColor.r + 0.1f, normalBgColor.g + 0.1f, normalBgColor.b + 0.1f, normalBgColor.a) :
                normalBgColor;
            bgImage.color = Color.Lerp(bgImage.color, bgTarget, Time.deltaTime * fadeSpeed);
        }
        */

        
    Vector3 targetScale = isHovered ? originalScale * 1.05f : originalScale;
    transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

    // 🔒 PROTECCIÓN CLAVE
    if (buttonText == null) return;

    if (isHovered)
    {
        buttonText.color = Color.Lerp(buttonText.color, highlightTextColor, Time.deltaTime * fadeSpeed);

        if (outlineEffect != null)
        {
            outlineEffect.enabled = true;
            outlineEffect.effectColor = new Color(0f, 0f, 0f, 1f);
        }
    }
    else
    {
        buttonText.color = Color.Lerp(buttonText.color, normalTextColor, Time.deltaTime * fadeSpeed);

        if (outlineEffect != null)
        {
            outlineEffect.effectColor = new Color(0f, 0f, 0f, 0.8f);
        }
    }
}


    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        // Efecto de sonido opcional (puedes agregarlo después)
        // AudioManager.PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    // Método para actualizar el color naranja dinámicamente
    public void UpdateOrangeColor(Color newOrangeColor)
    {
        darkOrange = newOrangeColor;
        CreateGradientTexture();

        if (glowOverlayImage != null && gradientMaterial != null)
        {
            glowOverlayImage.material = gradientMaterial;
        }
    }

    void OnDestroy()
    {
        // Limpiar texturas y materiales
        if (gradientTexture != null)
        {
            DestroyImmediate(gradientTexture);
        }
        if (gradientMaterial != null)
        {
            DestroyImmediate(gradientMaterial);
        }
    }

    // Implementación de IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        // Opcional: cambiar color al presionar
        if (bgImage != null)
        {
            // bgImage.color = new Color(darkOrange.r, darkOrange.g, darkOrange.b, 0.7f);
        }
    }

    // Implementación de IPointerUpHandler
    public void OnPointerUp(PointerEventData eventData)
    {
        // Volver al estado hover o normal
        if (bgImage != null)
        {
            // bgImage.color = isHovered ? darkOrange : normalBgColor;
        }
    }
}