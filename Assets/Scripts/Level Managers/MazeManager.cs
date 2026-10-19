    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MazeManager : MonoBehaviour
    {
        private bool _canSpawnWeight = true, _alreadyEnabled = false;
        public bool hasMazeStarted = false;
        public static MazeManager Instance {get; private set;}
        public SceneDataSO data; 

        [Header("Collections")]
        public List<NeutralNPC> mazeMisers = new();
        public List<Weight> weightInstances = new();
        public List<Trail> mazeTrails = new();

        [Header("Factories & Pools")]
        [SerializeField] private WeightFactory _weightFactory;
        public CreateWeightPool weightPoolManager;
        public CreateTrailPool trailPoolManager;
        [SerializeField] private TrailFactory _trailFactory;

        [Header("Tutorial Settings")]
        public NeutralNPC tutorialStarter;
        [SerializeField] private Transform _weightSpawnPoint, _tutorialWaypoint;
        [SerializeField] private Rigidbody _tutorialRockRB;
        [SerializeField] private GameObject[] _tutorialBarriers;
        public TutorialManager tutorial;

        private void Awake() 
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            trailPoolManager = new CreateTrailPool(_trailFactory);
            EventManager.Subscribe("OnTutorialEnded", SetAllMisers);
            EventManager.Subscribe("OnIntroEnded", x => data.firtsLoadDone = true);
        }

        private void Start() 
        {
            tutorial = new TutorialManager(StartCoroutine, 
                        _tutorialRockRB, 
                        _tutorialWaypoint, 
                        tutorialStarter,
                        _tutorialBarriers);

            //EventManager.Subscribe("PoolWeight", AudioManager.Instance.PlaySFXAt);
            StartCoroutine(FillList());
        }

        private void Update() 
        {
            List<Trail> copiedList = new List<Trail>(mazeTrails);

            foreach(Trail trail in copiedList)
            {
                if(trail == null || !trail.gameObject.activeInHierarchy)
                {
                    Debug.Log("Skipping null or inactive trail instance.");
                    continue;
                }

                trail.ArtificialUpdate();
            }
        }

        private void FixedUpdate() 
        {
            List<Weight> copiedList = new List<Weight>(weightInstances);
            
            foreach(Weight weight in copiedList)
            {
                if(weight == null || !weight.gameObject.activeInHierarchy)
                {
                    Debug.Log("Skipping null or inactive weight instance.");
                    continue;
                }

                weight.ArtificialUpdate();
            }

            if(GameManager.Instance.coolDownTimer <= 0 && !_canSpawnWeight) _canSpawnWeight = true;
        }

        public void SetAllMisers(params object[] parameters)
        {
            foreach(var item in parameters)
            {
                if(item is bool value)
                {
                    foreach(NeutralNPC item2 in mazeMisers)
                    {
                        GameManager.Instance.ActiveOrDesactive(item2.gameObject, value);

                        if(item2.gameObject.name == "Tutorial Miser")
                        {
                            item2.dialogue.sentences = new string[] {"...", "¡Maldito seas, que turbas mi efímero reposo!"};
                        }
                    }

                    break;
                }
            }
        }

        public void EnableTutorial()
        {
            if(_alreadyEnabled) return;

            weightPoolManager = new CreateWeightPool(_weightFactory, _weightSpawnPoint);

            List<NeutralNPC> copiedList = new List<NeutralNPC>(mazeMisers);
            foreach(NeutralNPC item in copiedList)
            {
                if(item.gameObject.name == "Tutorial Miser")
                {
                    item.isDialoguer = false;
                    item.isMazeMiser = true;
                    item.SetMazeMiser(true);
                    item.isComentarist = true;
                    item.dialogue.sentences = new string[]{"Muéstrame el camino ¡Oh tú, quien conserva la sabiduría!",
                                            "El peso agobia mi juicio; muéstrame hacia dónde debo empujar.",
                                            "Empujaré según tu voluntad; dime tan sólo el lugar.",
                                            "Aquí me encuentro perdido y agotado; ilumíname tú, quien recorre el camino de la salvación.",
                                            "Acataré tu mandato; sólo te ruego que me recuerdes si alcanzas el perdón.",
                                            "¡Sálvanos!, pues de tu victoria seremos parte.",
                                            "¡Ten piedad de nosotros!, que en tu hazaña dejamos nuestra huella."};
                }
            }

            tutorialStarter.dialogue.sentences = new string[] {"Haz que estos pecadores se entreguen a este peso."};
            ObjectiveManager.Instance.CompleteObjective();
            ObjectiveManager.Instance.SetObjective("Abre el conducto hacia el laberinto.", false);
            _alreadyEnabled = true;
        }

        public void PoolWeight(string type = "")
        {
            if(_canSpawnWeight)
            {
                if(type != "")
                    weightPoolManager.GetType(type);
        
                EventManager.Trigger("PoolWeight", false, 
                                    true, 
                                    new AudioClipData("WeightSpawned"), 
                                    new AudioClipData("MazeVoice2", tutorialStarter.transform.position));
                GameManager.Instance.coolDownTimer = 7f;
                _canSpawnWeight = false;
            }
        }

        private IEnumerator FillList()
        {
            yield return new WaitForSeconds(1f);

            foreach(Weight item in weightInstances)
            {
                if(item.gameObject.name == "Tutorial Weight")
                {
                    item.ResetPhysics();
                    item.GetComponent<Rigidbody>().isKinematic = true;
                    item.enabled = false;
                } 
            }
        }
    }
