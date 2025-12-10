using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PersonagemUI
{
    public string id;
    public Image retrato;
    public Transform raiz;
}

public class RealcadorDePersonagem : MonoBehaviour
{
    #region ==== CONFIG ====
    public List<PersonagemUI> personagens;

    [Header("Aparência")]
    [Range(0f, 1f)] public float escurecer = 0.4f; // 0 = preto total, 1 = normal
    public float escalaAtivo = 1.05f;
    public float escalaInativo = 1f;
    public float velocidadeTransicao = 6f;
    #endregion

    Dictionary<string, Coroutine> corrotinas = new Dictionary<string, Coroutine>();

    #region ==== DESTACAR PERSONAGEM ATIVO ====
    public void Destacar(string idAtivo)
    {
        foreach (var p in personagens)
        {
            bool ativo = p.id == idAtivo;

            // COR CERTA: escurecer puxando pro preto, sem transparência
            Color alvoCor = ativo ?
                Color.white :
                new Color(escurecer, escurecer, escurecer, 1f);

            float alvoEscala = ativo ? escalaAtivo : escalaInativo;

            if (corrotinas.ContainsKey(p.id) && corrotinas[p.id] != null)
                StopCoroutine(corrotinas[p.id]);

            corrotinas[p.id] = StartCoroutine(AnimarPersonagem(p, alvoCor, alvoEscala));
        }
    }
    #endregion

    #region ==== ANIMAÇÃO DE COR + ESCALA ====
    IEnumerator AnimarPersonagem(PersonagemUI p, Color alvoCor, float alvoEscala)
    {
        Image img = p.retrato;
        Transform t = p.raiz;

        Color corInicial = img.color;
        float escalaInicial = t.localScale.x;

        float tempo = 0;

        while (tempo < 1)
        {
            tempo += Time.deltaTime * velocidadeTransicao;

            img.color = Color.Lerp(corInicial, alvoCor, tempo);

            float s = Mathf.Lerp(escalaInicial, alvoEscala, tempo);
            t.localScale = new Vector3(s, s, 1);

            yield return null;
        }

        img.color = alvoCor;
        t.localScale = new Vector3(alvoEscala, alvoEscala, 1);
    }
    #endregion
}
