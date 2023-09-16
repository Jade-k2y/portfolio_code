using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Studio.Game
{
    public class UIStoryStage : UIScenePlay<StoryStageScriptable>
    {
        private Queue<ITutorial> _tutorials = new();


        private void Awake()
        {
            var tutorials = GetComponents<ITutorial>()
                ?.Where(x => !x.isDone && !x.useTrigger)
                ?.OrderBy(x => x.order);

            foreach (var tutorial in tutorials)
            {
                _tutorials.Enqueue(tutorial);
            }
        }


        protected override void Start()
        {
            base.Start();

            if (0 < _tutorials.Count)
            {
                StartCoroutine(RunTutorials());
            }
        }


        private IEnumerator RunTutorials()
        {
            while (0 < _tutorials.Count)
            {
                yield return _tutorials.Dequeue().Play();
            }
        }


        public void SetStoryStageScriptable(StoryStageScriptable scriptable)
        {
            if (_scriptable = scriptable)
            {
                if (_screen)
                {
                    _scriptable.SetTitle(_screen.title);
                }

                _scriptable.SetTitle(_stageInfo?.title);
                _scriptable.InitPhaseProgress(_stageInfo);
            }
        }
    }
}