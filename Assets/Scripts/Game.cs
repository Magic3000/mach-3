using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int dotVariants = 3;
    public int maxDestroyRange = 2;
    public int scoreToLeaderboard = 10;
    public GameObject dotsParent;
    public GameObject[] prefabs;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverScreen;
    public TextMeshProUGUI gameOverText;
    public Texture bananaTexture;
    public Texture cherriesTexture;
    public Texture grapesTexture;
    public Texture orangeTexture;
    public Texture strawberryTexture;
    public Texture watermelonTexture;
    public GameObject pauseButton;
    public GameObject pauseScreen;
    int score = 0;
    int moves = 3;
    List<GameObject> dots = new List<GameObject>();
    System.Random rnd = new System.Random();
    int maxY;
    List<Dot.DotType> usedDotTypes;
    GameObject GenerateDot(int x, int y)
    {
        var type = usedDotTypes[rnd.Next(0, usedDotTypes.Count)];
        var newDot = Instantiate(prefabs.FirstOrDefault(x => x.name.Contains(type.ToString().ToLower())), Vector3.zero, Quaternion.identity, dotsParent.transform);
        var pos = new Vector2(x * 200, y * 200);
        newDot.GetComponent<RectTransform>().localPosition = pos;

        var dot = newDot.GetComponent<Dot>();
        dot.dotType = type;
        var tempX = x;
        var tempY = y;
        dot.X = tempX;
        dot.Y = tempY;

        var btn = newDot.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            OnDotClick(dot);
        });
        dots.Add(newDot);
        return newDot;
    }
    void Start()
    {
        usedDotTypes = new List<Dot.DotType>();
        var dotValues = Enum.GetValues(typeof(Dot.DotType));
        if (dotVariants > dotValues.Length)
            dotVariants = dotValues.Length;
        for (int i = 0; i < dotVariants; i++)
        {
            var tempType = (Dot.DotType)dotValues.GetValue(rnd.Next(dotValues.Length));
            while (usedDotTypes.Contains(tempType))
                tempType = (Dot.DotType)dotValues.GetValue(rnd.Next(dotValues.Length));
            usedDotTypes.Add(tempType);
        }
        for (int x = -2; x < 3; x++)
        {
            for (int y = -3; y < 4; y++)
            {
                GenerateDot(x, y);
                maxY = y;
            }
        }
        UpdateScore();
    }

    // Part of old destroying algorithm
    /*bool IsOtherTypeDotBetween(Dot src, Dot dest)
    {
        var srcX = src.X;
        var srcY = src.Y;
        var destX = dest.X;
        var destY = dest.Y;
        bool result = false;
        foreach (var go in dots)
        {
            var dot = go.GetComponent<Dot>();
            if (dot.X > srcX && dot.X < destX && src.dotType != dot.dotType)
                result = true;
            else if (dot.X < srcX && dot.X > destX && src.dotType != dot.dotType)
                result = true;
            else if (dot.Y > srcY && dot.Y < destY && src.dotType != dot.dotType)
                result = true;
            else if (dot.Y < srcY && dot.Y > destY && src.dotType != dot.dotType)
                result = true;
        }
        Debug.Log($"{srcX}:{srcY} - {destX}:{destY} - {result}");
        return result;
    }*/

    Texture GetTextureByType(Dot.DotType type)
    {
        switch (type)
        {
            case Dot.DotType.Banana: return bananaTexture;
            case Dot.DotType.Cherries: return cherriesTexture;
            case Dot.DotType.Grapes: return grapesTexture;
            case Dot.DotType.Orange: return orangeTexture;
            case Dot.DotType.Strawberry: return strawberryTexture;
            case Dot.DotType.Watermelon: return watermelonTexture;
        }
        return null;
    }
    Dot MoveOnTop(Dot dot, bool changeType = false)
    {
        if (changeType)
        {
            var newType = usedDotTypes[rnd.Next(0, usedDotTypes.Count)];
            while (newType == dot.dotType)
                newType = usedDotTypes[rnd.Next(0, usedDotTypes.Count)];
            dot.dotType = newType;
            dot.gameObject.name = $"DotPrefab_{dot.dotType.ToString().ToLower()}(Clone)";
            dot.GetComponent<RawImage>().texture = GetTextureByType(dot.dotType);
        }
        dot.Y = maxY;
        var pos = dot.GetComponent<RectTransform>().localPosition;
        pos.y = dot.Y * 200;
        dot.GetComponent<RectTransform>().localPosition = pos;
        return dot;
    }
    Dot MoveTo(Dot dot, int Xnum = 0, int Ynum = 0, bool generateNew = false)
    {
        if (generateNew)
        {
            var x = dot.X + Xnum;
            var y = dot.Y + Ynum;
            Destroy(dot.gameObject);
            dot = GenerateDot(x, y).GetComponent<Dot>();
            return dot;
        }
        else
        {
            dot.X += Xnum;
            dot.Y += Ynum;
            var pos = dot.GetComponent<RectTransform>().localPosition;
            pos.x = dot.X * 200;
            pos.y = dot.Y * 200;
            dot.GetComponent<RectTransform>().localPosition = pos;
            return dot;
        }
    }

    Dot GetDotOfType(Dot srcDot, int x = 0, int y = 0)
    {
        foreach (var go in dots)
        {
            var dot = go.GetComponent<Dot>();
            if (dot.X == srcDot.X + x
                && dot.Y == srcDot.Y + y
                && dot.dotType == srcDot.dotType)
                return dot;
        }
        return null;
    }
    
    public void OnDotClick(Dot selfDot)
    {
        Debug.Log($"OnDotClick {selfDot.X}:{selfDot.Y}");
        var selfType = selfDot.dotType;
        var destroyedDots = new List<Dot>() { selfDot };

        for (int i = 0; i < maxDestroyRange; i++)
        {
            var dotLeft = GetDotOfType(selfDot, -1 + (i * -1), 0);
            if (dotLeft != null)
                destroyedDots.Add(dotLeft);
            else
                break;
        }

        for (int i = 0; i < maxDestroyRange; i++)
        {
            var dotRight = GetDotOfType(selfDot, 1 + i, 0);
            if (dotRight != null)
                destroyedDots.Add(dotRight);
            else
                break;
        }
        
        for (int i = 0; i < maxDestroyRange; i++)
        {
            var dotTop = GetDotOfType(selfDot, 0, 1 + i);
            if (dotTop != null)
                destroyedDots.Add(dotTop);
            else
                break;
        }

        for (int i = 0; i < maxDestroyRange; i++)
        {
            var dotBottom = GetDotOfType(selfDot, 0, -1 + (i * -1));
            if (dotBottom != null)
                destroyedDots.Add(dotBottom);
            else
                break;
        }
        
        // Old destroying algorithm  
        /*foreach (var go in dots)
        {
            var dot = go.GetComponent<Dot>();
            var dotX = dot.X;
            var dotY = dot.Y;
            if (selfDot.X - maxDestroyRange <= dotX && selfDot.X > dotX 
                && selfDot.Y == dotY && selfType == dot.dotType && !IsOtherTypeDotBetween(selfDot, dot))
            {
                destroyedDots.Add(dot);
            }
            else if (selfDot.X + maxDestroyRange >= dotX && selfDot.X < dotX
                && selfDot.Y == dotY && selfType == dot.dotType && !IsOtherTypeDotBetween(selfDot, dot))
            {
                destroyedDots.Add(dot);
            }
            else if (selfDot.Y - maxDestroyRange <= dotY && selfDot.Y > dotY
                && selfDot.X == dotX && selfType == dot.dotType && !IsOtherTypeDotBetween(selfDot, dot))
            {
                destroyedDots.Add(dot);
            }
            else if (selfDot.Y + maxDestroyRange >= dotY && selfDot.Y < dotY
                && selfDot.X == dotX && selfType == dot.dotType && !IsOtherTypeDotBetween(selfDot, dot))
            {
                destroyedDots.Add(dot);
            }
        }*/

        var dotsCopy = new List<GameObject>();
        dotsCopy.AddRange(dots);
        for (int i = 0; i < destroyedDots.Count; i++)
        {
            var dotToDestroy = destroyedDots[i];
            for (int j = 0; j < dots.Count; j++)
            {
                var dot = dots[j].GetComponent<Dot>();
                if (dot.Y > dotToDestroy.Y && dot.X == dotToDestroy.X)
                {
                    var movedDot = MoveTo(dot, 0, -1);
                    dots[j] = movedDot.gameObject;
                }
                /*else if (dot.X == dotToDestroy.X 
                    && dot.Y == dotToDestroy.Y)
                {
                    dots[j] = MoveOnTop(dotToDestroy, true).gameObject;
                }*/
            }
            MoveOnTop(dotToDestroy, true);
        }
        /*foreach (var dotToDestroy in destroyedDots)
        {
            foreach (var go in dots)
            {
                var dot = go.GetComponent<Dot>();
                if (dot.Y > dotToDestroy.Y && dot.X == dotToDestroy.X)
                {
                    dot.Y--;
                    var pos = dot.GetComponent<RectTransform>().localPosition;
                    pos.y = dot.Y * 200;
                    dot.GetComponent<RectTransform>().localPosition = pos;
                }
            }
            dotToDestroy.Y = maxY * 200;
            var destroyedPos = dotToDestroy.GetComponent<RectTransform>().localPosition;
            destroyedPos.y = dotToDestroy.Y;
            dotToDestroy.GetComponent<RectTransform>().localPosition = destroyedPos;
            //dots.Remove(dot.gameObject);
            //Destroy(dot.gameObject);
        }*/

        Debug.Log($"OnDotClick {selfDot.X}:{selfDot.Y} {selfType} destroyedDots: {destroyedDots.Count}");

        moves--;
        score += destroyedDots.Count;
        if (destroyedDots.Count > 2)
        {
            moves += destroyedDots.Count - 1;
        }
        UpdateScore();
    }

    void UpdateScore()
    {
        if (moves == 0)
        {
            GameOver();
            return;
        }
        scoreText.text = $"Score: {score}\nMoves: {moves}";
    }
    void GameOver()
    {
        dots.Clear();
        Destroy(dotsParent);
        scoreText.gameObject.SetActive(false);
        if (score >= scoreToLeaderboard)
        {
            Leaderboard.SaveScore(score);
            SceneManager.LoadScene("Leaderboard");
        }
        else
        {
            gameOverScreen.SetActive(true);
            gameOverText.text = $"Game Over :(\nScore: {score}";
        }
    }

    public void OpenPauseMenu()
    {
        pauseButton.SetActive(false);
        dotsParent.SetActive(false);
        pauseScreen.SetActive(true);
    }
    public void ClosePuuseMenu()
    {
        pauseButton.SetActive(true);
        dotsParent.SetActive(true);
        pauseScreen.SetActive(false);
    }
    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
