    /*
        TODO: 
        - Create child canvases
        - Levels - DONE
        - Fix levels - DONE 
        - Add bad colors screen - DONE  - MAKE THESE UNCLICKABLE
        - Restart screen - DONE - DISABLE TOUCH INPUT ON DEATH
        - Social Media on Settings Screen - DONE
        - Add sound - IGNORE for now
        - Instantiate dots randomly on start screen
        - boundaries for dots - Partially DONE (need to fix)
        - change tap to start and restart to buttons - DONE
        - Add highscore - DONE
        - Add icons/3d spheres - DONE
        - Handle diferent resolutions
        - Constrict to vertical orientation
        - BUGS:
            - Fix tap to start not working sometimes.
        */

        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        using UnityEngine.SceneManagement;
        using UnityEngine.UI;
        using TMPro;

        public class DotHandler : MonoBehaviour
        {
            GameObject[] initialDots;
            GameObject[] initialObjects;
            private bool levelPassed;

            private float width = (float)Screen.width;
            private float height = (float)Screen.height;
            private float adjustedWidth;
            private float adjustedHeight;
            private Vector3 adjustedDimensions;
            private bool dotsPlaced=true;
            private bool gameStarted=true;
            public GameObject mainDot;
            public GameObject blueDot;
            public GameObject greenDot;
            public GameObject redDot;
            public GameObject yellowDot;
            public GameObject purpleDot;
            public GameObject orangeDot;
            public GameObject canvas;
            public GameObject pausedIcon;
            public GameObject vibrateOn;
            public GameObject vibrateOff;
            public GameObject soundOn;
            public GameObject soundOff;
            public GameObject rulesPanel;
            //contains list of ALL dots
            private GameObject[] dots;
            private bool startTapFlag=false;
            private int index=0;
            public TextMeshProUGUI highscoreText;
            public TextMeshProUGUI highestScoreText;
            public TextMeshProUGUI deathPoints;
            public TextMeshProUGUI resumeTimerText;
            public TextMeshProUGUI badColorsText;
            public TextMeshProUGUI continueText;
            public GameObject restartText;
            public TextMeshProUGUI pointsText;
            public TextMeshProUGUI timerText;
            public TextMeshProUGUI badDotsText;
            public TextMeshProUGUI goodDotsText;
            private int points=0;
            private bool died=false;
            private string badColorFirst = "BlueDot";
            private string badColorSecond = "GreenDot";
            private bool pressed=false;
            //contains list of all "good" dots
            private List<GameObject> dotsList = new List<GameObject>();
            private List<GameObject> badDots = new List<GameObject>();
            private List<Color> randomColors = new List<Color>();
            public GameObject pausePanel;
            public GameObject pauseButton;
            private bool dotHit=false;
            // Start is called before the first frame update
            private bool paused=false;
            public float timer = 5.0f;
            private float timerAnchor = 5.0f;
            private bool stopped = false;
            public GameObject settingsPanel;
            private bool settingsActive = false;
            private float maxTimer = 5.0f;
            private bool levelPaused = false;
            private bool gameBegan = false;
            private bool called = false;
            private int badColorIndexF;
            private int badColorIndexS;
            private bool initial = true;
            private bool screenSwitch = false;
            public int highScore = 0;
            private bool startClicked = false;
            private List<GameObject> dotsClone = new List<GameObject>();
            private List<GameObject> dotsListClone = new List<GameObject>();
            private List<GameObject> badDotsClone = new List<GameObject>();
            private List<Sprite> backgrounds = new List<Sprite>();
            public GameObject mainCanvas;
            private float resumeTimer = 3;
            private bool vibrateFlag = true;
            private bool soundFlag = true;
            float currentTime = 0f;
            float timeToMove = 2f;
            private bool moving = true;
            public FocusSwitcher focus;
            public AudioSource audioSource;
            public AudioSource popAudio;
            private bool readRules = false;
            private bool rulesActive = false;
            public Sprite blueBG;
            public Sprite bluePurpleBG;
            public Sprite purpleBG;
            public Sprite redBG;
            public Sprite sunsetBG;
            void Start()
            {
                backgrounds.Add(blueBG);
                backgrounds.Add(bluePurpleBG);
                backgrounds.Add(purpleBG);
                backgrounds.Add(redBG);
                backgrounds.Add(sunsetBG);

                int randomIndex = Random.Range(0,4);
                mainCanvas.GetComponent<Image>().sprite = backgrounds[randomIndex];

                adjustedDimensions = Camera.main.ScreenToWorldPoint(new Vector3(width, height, 0));
                adjustedWidth = adjustedDimensions.x;
                adjustedHeight = adjustedDimensions.y;
                initialObjects = GameObject.FindGameObjectsWithTag("Initial");
                dots = new GameObject[] {blueDot,greenDot,redDot,yellowDot,purpleDot,orangeDot};
                //randomizes the order of the dots and selects two "bad" colors
                Debug.Log("initial: "+initial);
                
                randomizeArray();           
               // badColorsText.text = "Bad Colors: "+badColorFirst+" and "+badColorSecond;
                pointsText.text = "0";
                initial = false;
                StartCoroutine(RandomDotColor());
                highScore = PlayerPrefs.GetInt("HighScore", highScore);
                readRules = PlayerPrefs.GetInt("ReadRules", 0) == 1 ? true : false;
                readRules=false;
                highestScoreText.text = highScore.ToString();
                highscoreText.text = "High Score: "+highScore.ToString();
                Time.timeScale = 1;
                
                randomColors.Add(blueDot.GetComponent<SpriteRenderer>().material.color);
                randomColors.Add(greenDot.GetComponent<SpriteRenderer>().material.color);
                randomColors.Add(redDot.GetComponent<SpriteRenderer>().material.color);
                randomColors.Add(yellowDot.GetComponent<SpriteRenderer>().material.color);
                randomColors.Add(purpleDot.GetComponent<SpriteRenderer>().material.color);
                randomColors.Add(orangeDot.GetComponent<SpriteRenderer>().material.color);

            }
            // Update is called once per frame
           
            void Update()
            {
                if(Input.touchCount>0&&Input.GetTouch(0).phase==TouchPhase.Began&&paused){
                    pausedIcon.gameObject.SetActive(false);
                    resumeGame();
                }
                if(Input.touchCount>0&&Input.GetTouch(0).phase==TouchPhase.Began&&screenSwitch){             
                    foreach(GameObject dot in dotsListClone){
                        Destroy(dot);
                    }
                    foreach(GameObject badDot in badDotsClone){
                        Destroy(badDot);
                    }   
                    audioSource.Play();
                    continueText.gameObject.SetActive(false);
                    StartGame();
                    screenSwitch = false;
                }     

                //detect touch when the game is running
                if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
                    Vector3 point = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(point, Vector3.zero);
                    string clickedDot="";
                    if(hit.collider!=null){
                        clickedDot = hit.transform.gameObject.name;
                    }
                    if(clickedDot.Contains("Dot")){
                        dotHit=true;
                    }
                    if(clickedDot.Equals("Pause")&&gameBegan){
                        pauseGame();
                    }
                    if(clickedDot.Equals("ResumeButton")){
                        //resumeGame();
                    }
                    if(clickedDot.Equals("Settings")){
                        Debug.Log("Settings pressed");
                        settingsActive = true;
                        settingsPanel.SetActive(true);
                    }
                    if(rulesActive&&hit.collider==null){
                        rulesActive = false;
                        rulesPanel.gameObject.SetActive(false);
                        badDotsText.gameObject.SetActive(true);
                        goodDotsText.gameObject.SetActive(true);
                        screenSwitch=true;
                        CategoryDotsSpawn();
                    }
                    if(!gameBegan&&!screenSwitch&&!settingsActive&&!died&&hit.collider==null){
                        StopCoroutine(DotColorChange());
                        Debug.Log("OK");
                        setInitialVisibility(false);
                        audioSource.Play();
                        if(!readRules){
                            rulesActive = true; 
                            rulesPanel.gameObject.SetActive(true);
                            continueText.gameObject.SetActive(true);
                        }else{
                            continueText.gameObject.SetActive(true);
                            badDotsText.gameObject.SetActive(true);
                            goodDotsText.gameObject.SetActive(true);
                            screenSwitch=true;
                            CategoryDotsSpawn();
                        }
                    }
                    if(!gameBegan&&died&&!levelPaused&&!paused&&hit.collider==null){
                        Scene scene = SceneManager.GetActiveScene();
                        SceneManager.LoadScene(scene.name);
                        gameBegan = false;
                        audioSource.Play();
                    }

                    if(clickedDot.Equals("Website")){
                        Application.OpenURL("https://watercolorgames.com/");
                    }
                    if(clickedDot.Equals("DisableRules")){
                        Debug.Log("Disable Rules");
                        PlayerPrefs.SetInt("ReadRules", 1);
                        rulesPanel.gameObject.SetActive(false);
                        badDotsText.gameObject.SetActive(true);
                        goodDotsText.gameObject.SetActive(true);
                        screenSwitch=true;
                        CategoryDotsSpawn();
                    }
                    Debug.Log("INDEX"+index);
                    if(hit.collider != null  && index<4 && dotHit && clickedDot.Contains(dotsClone[index].name)&&gameBegan&&!screenSwitch&&!paused)
                    {
                        Debug.Log("Dot Clicked : " + dots[index].name);
                        Destroy(hit.transform.gameObject);
                        index++;
                        points++;
                        pointsText.text = points.ToString();
                        //Debug.Log("Index"+index);
                        popAudio.Play();

                    }
                    else if(dotHit&&!screenSwitch&&gameBegan){
                        Debug.Log("Dot Clicked :" + dots[index].name);
                        StopCoroutine(DotTimeGap());

                        restartText.gameObject.SetActive(true);
                        //highscoreText.gameObject.SetActive(true);
                        
                        if(points>highScore){
                            PlayerPrefs.SetInt("HighScore",points);
                            highScore = points;
                            highscoreText.text = "Highest Score: "+points.ToString();
                        }
                        gameBegan=false;
                        died = true;
                        timerText.gameObject.SetActive(false);
                        pointsText.gameObject.SetActive(false);
                        pauseButton.gameObject.SetActive(false);
                        deathPoints.gameObject.SetActive(true);
                        deathPoints.text = points.ToString();
                        focus.SetFocused(deathPoints.gameObject);
                        focus.SetFocused(restartText.gameObject);
                        StartCoroutine(ZoomCamera(Camera.main.orthographicSize, 10, 2, 75));
                    }
                };

                // detect touch when the settings panel is open
                if(Input.touchCount>0&&Input.GetTouch(0).phase==TouchPhase.Began&&settingsActive){
                    Vector3 point = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    RaycastHit2D hit = Physics2D.Raycast(point, Vector3.zero);
                    string clickedDot="";
                    if(hit.collider!=null){
                        clickedDot = hit.transform.gameObject.name;
                    }else{
                        settingsActive = false;
                        settingsPanel.SetActive(false);
                    }
                    if(clickedDot.Equals("SettingsClose")){
                        Debug.Log("Settings Close pressed");
                        settingsActive = false;
                        settingsPanel.SetActive(false);
                    }
                    if(clickedDot.Equals("Instagram")){
                        Application.OpenURL("https://www.instagram.com/watercolorgames/");
                    }  
                    if(clickedDot.Equals("Linkedin")){
                        Application.OpenURL("https://www.linkedin.com/company/watercolorgames?trk=public_profile_topcard-current-company");
                    }    
                    if(clickedDot.Equals("VibrateOn")){
                        vibrateOn.gameObject.SetActive(false);
                        vibrateOff.gameObject.SetActive(true);
                        vibrateFlag = false;
                    }if(clickedDot.Equals("VibrateOff")){
                        vibrateOn.gameObject.SetActive(true);
                        vibrateOff.gameObject.SetActive(false);
                        vibrateFlag = true;
                    }
                    if(clickedDot.Equals("SoundOn")){
                        soundOn.gameObject.SetActive(false);
                        soundOff.gameObject.SetActive(true);
                        soundFlag = false;
                    }if(clickedDot.Equals("SoundOff")){
                        soundOn.gameObject.SetActive(true);
                        soundOff.gameObject.SetActive(false);
                        soundFlag = true;
                    }
                    
                    

                }  

                //if all dots are pressed, screen clears, dots are randomized, and the game continues
                if(index==4&&gameBegan){

                    StopCoroutine(DotTimeGap());
                    if(badDots.Count>0){
                        Destroy(badDots[0]);
                        Destroy(badDots[1]);
                    }
                    if(vibrateFlag){
                        Handheld.Vibrate();
                    }
                    pauseButton.gameObject.SetActive(false);
                    pointsText.gameObject.SetActive(false);
                    timerText.gameObject.SetActive(false);
                    levelPaused = true;

                    if(!called){
                        Invoke("DotTimeGapLvl", 2f);
                    }
                    called = true;
  
                }

                // if timer runs out, the game restarts
                if(!levelPaused){
                    if(timer<=0){
                        //highscoreText.gameObject.SetActive(true);
                        restartText.gameObject.SetActive(true);
                        if(points>highScore){
                            PlayerPrefs.SetInt("HighScore",points);
                            highScore = points;
                            highscoreText.text = points.ToString();
                        }
                        //Time.timeScale = 0;
                        //find a way to stop user input here    
                        //Time.timeScale = 0;
                        //Debug.Log("Timer ran");
                        //restartText.gameObject.SetActive(true);
                        StopCoroutine(DotTimeGap());
                        gameBegan=false;
                        died = true;
                        timerText.gameObject.SetActive(false);
                        pointsText.gameObject.SetActive(false);
                        pauseButton.gameObject.SetActive(false);
                        deathPoints.gameObject.SetActive(true);
                        deathPoints.text = points.ToString();
                        focus.SetFocused(deathPoints.gameObject);
                        focus.SetFocused(restartText.gameObject);
                        StartCoroutine(ZoomCamera(Camera.main.orthographicSize, 10, 2, 75));
                        
                    }
                }

                
                //if the game starts, the timer counts down
                if(gameBegan){
                    timer -= Time.deltaTime;
                    if(timer>0){
                        timerText.text=timer.ToString("0.0");
                    }

                }
            
            }
            
            public void pauseGame(){
                //pausePanel.gameObject.SetActive(true);
                canvas.gameObject.SetActive(true);
                pausedIcon.gameObject.SetActive(true);
                paused = true;
                Time.timeScale = 0;
            }

            public void resumeGame(){
                stopped = true;
                StartCoroutine(ResumeWait());
            }

            //makes initial UI elements invisible/visible
            void setInitialVisibility(bool active){
                foreach (GameObject initObject in initialObjects)
                {
                    //Debug.Log("initial object: "+initObject.name);
                    initObject.gameObject.SetActive(active);
                }
            
            }

            //randomizes order of dots in the array
            private void randomizeArray(){
                dotsList.Clear();
                dotsClone.Clear();
                //dots = new GameObject[] {blueDot,greenDot,redDot,yellowDot,purpleDot,orangeDot};
                System.Random rnd = new System.Random();
                //Debug.Log("FIRST DOT NAME"+dots[0].name);

                for (int i = 0; i < dots.Length; i++) 
                {
                    //Debug.Log("dot name"+dots[i].name  );
                    int r = rnd.Next(i, dots.Length);
                    (dots[r], dots[i]) = (dots[i], dots[r]);
                }
                if(initial){
                    randomizeBadColors();
                }
                //randomizeBadColors();
                for(var i = 0; i < dots.Length; i++)
                {
                    Debug.Assert(dots[i] != null, $"Array entry is null at index {i} ", gameObject);
                 }
                
                for(int i=0;i<dots.Length;i++){
                    if(!(dots[i].name.Contains(badColorFirst))&&!(dots[i].name.Contains(badColorSecond))){
                        dotsList.Add(dots[i]);
                        dotsClone.Add(dots[i]);
                    }else{
                        badDots.Add(dots[i]);
                    }
                }
            }
            //randomizes the two "bad" colors once the game starts
            private void randomizeBadColors(){
                System.Random random = new System.Random();
                badColorIndexF = random.Next(0, dots.Length-1);
                badColorFirst = dots[badColorIndexF].name.ToString();
                badColorIndexS = random.Next(0, dots.Length-1);
                badColorSecond = dots[badColorIndexS].name.ToString();

                if(badColorFirst.Equals(badColorSecond)){
                    randomizeBadColors();
                }
                Debug.Log("bad color first: "+badColorFirst);
                Debug.Log("bad color second: "+badColorSecond);
            }
            //generates a random position for the dots to be placed
            private Vector3 generateRandomPosition(float xMin, float xMax, float yMin, float yMax){
                float x = UnityEngine.Random.Range((xMin),(xMax));
                float y = UnityEngine.Random.Range((yMin),(yMax));
                Vector3 dotSpawn = new Vector3(x,y,-1f);
                Collider2D overlap = Physics2D.OverlapCircle(dotSpawn,0.5f,LayerMask.GetMask("Default"));     
                if(overlap==null){
                    return new Vector3(x,y,-1);
                }else{
                    return Vector3.zero;
                }
            }

            //instantiates dots with time delay
            private IEnumerator DotTimeGap()
            {
                badDots.Clear();
                dotsList.Clear();
                while(paused){
                    yield return null;
                }
                
                Debug.Log("Number of dots to place: "+dots.Length);
                foreach (GameObject dot in dots)
                {
                    Vector3 randomPos = generateRandomPosition(-adjustedWidth+0.5f,adjustedWidth-0.5f,-adjustedHeight+0.5f,adjustedHeight-1.0f);
                    while(randomPos==Vector3.zero){
                        randomPos = generateRandomPosition(-adjustedWidth+0.5f,adjustedWidth-0.5f,-adjustedHeight+0.5f,adjustedHeight-1.0f);
                    }
                    GameObject a = (GameObject)Instantiate(dot,randomPos,Quaternion.identity);
                    if(a.name.Contains(badColorFirst)||a.name.Contains(badColorSecond)){
                        badDots.Add(a);
                    }else{
                        dotsList.Add(a);
                        Debug.Log("Dot Placed"+a.name);
                    }
                    //StartCoroutine(DotFade(a));
                    yield return new WaitForSeconds(.5f);
                }
            }            
            //when the player presses "resume", the game is paused for 10 seconds before the player can start
            private IEnumerator ResumeWait(){
                Debug.Log("disabling pause panel");
                //add functionality to stop recording touch activity
                resumeTimerText.gameObject.SetActive(true);
                resumeTimerText.text = resumeTimer.ToString("0");

                while(resumeTimer>0){
                    yield return new WaitForSecondsRealtime(1.0f);
                    resumeTimer--;
                    resumeTimerText.text = resumeTimer.ToString("0");

                }
                //yield return new WaitForSecondsRealtime(3.0f);
                pausePanel.gameObject.SetActive(false);
                canvas.gameObject.SetActive(false);
                resumeTimerText.gameObject.SetActive(false);
                paused = false;
                Time.timeScale = 1;
                stopped = false;
                resumeTimer = 3;
            }
            //dots spawn on good/bad dots screen
            private void CategoryDotsSpawn(){

                screenSwitch = true;
                float x = -1.85f;
                foreach(GameObject dot in dotsList){
                    Vector3 pos = new Vector3(x,-.435f,-1f);
                    GameObject a = (GameObject)Instantiate(dot,pos,Quaternion.identity);
                    dotsListClone.Add(a);
                    x+=1.25f;
                }
                x=-.65f;
                foreach(GameObject badDot in badDots){
                    Vector3 pos = new Vector3(x,-3.65f,-1);
                    GameObject b = (GameObject)Instantiate(badDot,pos,Quaternion.identity);
                    badDotsClone.Add(b);
                    x+=1.25f;
                }

            }
            //initializers for game beginning
            private void StartGame(){
                badDotsText.gameObject.SetActive(false);
                goodDotsText.gameObject.SetActive(false);
                pointsText.gameObject.SetActive(true);
                timerText.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(true);
                StartCoroutine(DotTimeGap());
                gameBegan = true;
            }
            //runs on level completion, and new dots are spawned
            private void DotTimeGapLvl(){
  
                timerAnchor-=.1f;
                timer = timerAnchor;    
                randomizeArray();
                index = 0;
                levelPaused = false;
                gameBegan= true ;

                pauseButton.gameObject.SetActive(true);
                pointsText.gameObject.SetActive(true);
                timerText.gameObject.SetActive(true);
                called = false;
                StartCoroutine(DotTimeGap());
            }
            //Zooms camera out on death
            IEnumerator ZoomCamera(float from, float to, float time, float steps)
            {
                float f = 0;
        
                while (f <= 1)
                {
                    Camera.main.orthographicSize = Mathf.Lerp(from, to, f);
        
                    f += 1f/steps;
        
                    yield return new WaitForSeconds(time/steps);
                }
            }
            //generates random color for dot in "DoT" title start screen
            private IEnumerator RandomDotColor(){
                while(!gameBegan&&!screenSwitch){
                    
                    int index = Random.Range(0,5);
                    float randomG = Random.Range(0.0f,1.0f);
                    float randomB = Random.Range(0.0f,1.0f);
                    mainDot.GetComponent<SpriteRenderer>().material.color =  randomColors[index];
                    yield return new WaitForSeconds(1.0f);
                }

            }
            // archive: dots spawn on start screen
            private IEnumerator DotColorChange(){
                while(!gameBegan&&!screenSwitch){
                    /*
                    float randomR = Random.Range(0.0f,1.0f);
                    float randomG = Random.Range(0.0f,1.0f);
                    float randomB = Random.Range(0.0f,1.0f);
                    mainDot.GetComponent<Renderer>().material.color = new Color(randomR,randomG,randomB);*/
                    int randomIndex = Random.Range(0,6);
                    float xFirst = UnityEngine.Random.Range((-adjustedWidth+0.5f),(adjustedWidth-0.5f));
                    float yFirst = UnityEngine.Random.Range((-3.78f),(0.46f));
                    Vector3 spawnPos  = generateRandomPosition(-adjustedWidth+0.5f,adjustedWidth-0.5f,-3.78f,0.46f);
                    while(spawnPos==Vector3.zero){
                        spawnPos = generateRandomPosition(-adjustedWidth+0.5f,adjustedWidth-0.5f,-3.78f,0.46f);
                    }
                    GameObject a = (GameObject)Instantiate(dots[randomIndex],spawnPos,Quaternion.identity);
                    a.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                    for(float i = a.transform.localScale.x;i<.8f; i+=.1f){
                        a.transform.localScale += new Vector3(0.05f,0.05f,0f);
                        if(screenSwitch){
                            StopCoroutine(DotColorChange());
                        }
                        yield return new WaitForSeconds(0.005f);  
                    }
                    StartCoroutine(DotFade(a));
                    randomIndex = Random.Range(0,6);
                    //x = UnityEngine.Random.Range((-adjustedWidth+0.5f),(adjustedWidth-0.5f));
                    //y = UnityEngine.Random.Range((-3.78f),(0.46f));
                    spawnPos = generateRandomPosition(-adjustedWidth+0.5f,adjustedWidth-0.5f,-3.78f,0.46f);
                    while(spawnPos==Vector3.zero){
                        spawnPos = generateRandomPosition(-adjustedWidth+0.5f,adjustedWidth-0.5f,-3.78f,0.46f);
                    }
                    GameObject b = (GameObject)Instantiate(dots[randomIndex],spawnPos,Quaternion.identity);
                    b.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                    for(float i = b.transform.localScale.x;i<.8f; i+=.1f){
                        b.transform.localScale += new Vector3(0.1f,0.1f,0f);
                        yield return new WaitForSeconds(0.1f);  
                    }
                    StartCoroutine(DotFade(b));
                    if(screenSwitch){
                        StopCoroutine(DotColorChange());
                    }
                    yield return new WaitForSeconds(.5f);  
                    Destroy(a);
                    Destroy(b);
                }

                yield return null;
            }
            // archive: dots fading in and out on start screen
            private IEnumerator DotFade(GameObject a){
            if(!screenSwitch){
                float targetAlpha = 0.0f;
                Color curColor = a.GetComponent<SpriteRenderer>().material.color;
                //Color curColor2 = b.GetComponent<SpriteRenderer>().color;
                Debug.Log("alpha"+a.GetComponent<SpriteRenderer>().material.color.a);
                while((float)a.GetComponent<SpriteRenderer>().material.color.a - targetAlpha > 0.0f) {
                    a.GetComponent<SpriteRenderer>().material.color = new Color(curColor.r, curColor.g, curColor.b, Mathf.Lerp(a.GetComponent<SpriteRenderer>().material.color.a, targetAlpha, 5.0f * Time.deltaTime));
                    //b.GetComponent<SpriteRenderer>().color = new Color(curColor2.r, curColor2.g, curColor2.b, Mathf.Lerp(b.GetComponent<SpriteRenderer>().color.a, targetAlpha, 5.0f * Time.deltaTime));
                    yield return null;
                }
                }else{
                    StopCoroutine(DotFade(a));
                }
            }
        }