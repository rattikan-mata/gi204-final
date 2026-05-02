using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AIQuestionService : MonoBehaviour
{
    public static AIQuestionService Instance { get; private set; }

    [Tooltip("ใส่ API Key จาก https://console.anthropic.com")]
    public string apiKey = "YOUR_API_KEY_HERE";
    public string model = "claude-haiku-4-5-20251001";
    public string topic = "Basic Physics: Force and Motion";

    [Serializable]
    public class QuestionData
    {
        public string question;
        public string[] choices;
        public int correctIndex;
    }

    private const string API_URL = "https://api.anthropic.com/v1/messages";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GetQuestion(string overrideTopic, Action<QuestionData, string> callback)
    {
        string useTopic = string.IsNullOrEmpty(overrideTopic) ? topic : overrideTopic;
        StartCoroutine(FetchQuestion(useTopic, callback));
    }

    private IEnumerator FetchQuestion(string useTopic, Action<QuestionData, string> callback)
    {
        string systemPrompt =
            "คุณคือระบบสร้างคำถามสำหรับเกมการศึกษา " +
            "ตอบกลับเป็น JSON เท่านั้น ห้ามมี markdown หรือข้อความอื่น " +
            "รูปแบบ JSON:\n" +
            "{\n" +
            "  \"question\": \"คำถาม\",\n" +
            "  \"choices\": [\"A\",\"B\",\"C\",\"D\"],\n" +
            "  \"correctIndex\": 0\n" +
            "}";

        string userPrompt =
            $"สร้างคำถาม {useTopic} 1 ข้อ " +
            "ระดับมัธยม ไม่ซ้ำกับคำถามทั่วไป " +
            "มี 4 ตัวเลือก ตัวเลือกต้องสมเหตุสมผลทุกข้อ " +
            "ตอบเป็น JSON ตามรูปแบบที่กำหนดเท่านั้น";

        string jsonBody =
            "{" +
            $"\"model\":\"{model}\"," +
            "\"max_tokens\":512," +
            $"\"system\":{JsonEscape(systemPrompt)}," +
            "\"messages\":[{\"role\":\"user\",\"content\":" + JsonEscape(userPrompt) + "}]" +
            "}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using UnityWebRequest req = new UnityWebRequest(API_URL, "POST");
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("x-api-key", apiKey);
        req.SetRequestHeader("anthropic-version", "2023-06-01");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("[AIQuestionService] Network error: " + req.error);
            callback?.Invoke(null, req.error);
            yield break;
        }

        string raw = req.downloadHandler.text;
        Debug.Log("[AIQuestionService] Raw response: " + raw);

        try
        {
            ClaudeResponse apiResp = JsonUtility.FromJson<ClaudeResponse>(raw);
            if (apiResp?.content == null || apiResp.content.Length == 0)
                throw new Exception("content array ว่างเปล่า");

            string questionJson = apiResp.content[0].text.Trim();

            if (questionJson.StartsWith("```"))
            {
                int start = questionJson.IndexOf('\n') + 1;
                int end = questionJson.LastIndexOf("```");
                questionJson = questionJson.Substring(start, end - start).Trim();
            }

            QuestionData q = JsonUtility.FromJson<QuestionData>(questionJson);
            if (q == null || q.choices == null || q.choices.Length < 4)
                throw new Exception("JSON ไม่ครบถ้วน");

            callback?.Invoke(q, null);
        }
        catch (Exception ex)
        {
            Debug.LogError("[AIQuestionService] Parse error: " + ex.Message);
            callback?.Invoke(null, ex.Message);
        }
    }

    private static string JsonEscape(string s) =>
        "\"" + s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "")
                .Replace("\t", "\\t") + "\"";

    [Serializable] private class ClaudeResponse { public ContentBlock[] content; }
    [Serializable] private class ContentBlock { public string text; }
}