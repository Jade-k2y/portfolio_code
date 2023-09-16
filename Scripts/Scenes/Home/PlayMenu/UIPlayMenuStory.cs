using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Studio.Game
{
    public class UIPlayMenuStory : UIPlayMenu
    {
        [SerializeField]
        private Image _thumbnail;
        [SerializeField]
        private Sprite _defaultBG;
        [SerializeField]
        private StoryCollectionScriptable.AssetReference _stories;
        [SerializeField]
        private TMP_Text _title, _content;

        private StoryCollectionScriptable _storiesAsset;

        public StoryCollectionScriptable storiesAsset => AssetScriptable.GetLoadAsset(_stories, ref _storiesAsset);


        private void OnDestroy()
        {
            if (_storiesAsset)
            {
                AssetScriptable.ReleaseAsset(_stories);
            }
        }


        protected override void Start()
        {
            base.Start();

            if (storiesAsset)
            {
                var storyIndex = Global<User>.instance.storyIndex;

                if (storiesAsset.TryGetStoryContent(storyIndex, out var story))
                {
                    if (story is StoryEpisodeScriptable episode)
                    {
                        episode.SetTitle(_title);

                        if (_thumbnail)
                        {
                            _thumbnail.sprite = _defaultBG;
                        }

                        SetContentText("스토리 에피소드");
                    }
                    else if (story is StoryStageScriptable stage)
                    {
                        stage.SetTitle(_title);
                        stage.SetThumbnail(_thumbnail);
                        /*
                        if (_thumbnail)
                        {
                            _thumbnail.color = _thumbnail.sprite ? new Color32(255, 255, 255, 180) : Color.white;
                        }
                        */
                        SetContentText("스토리 스테이지");
                    }
                }
                else
                {
                    if (_title)
                    {
                        _title.SetText($"CHAPTER 1-{storyIndex}");
                    }

                    if (_thumbnail)
                    {
                        _thumbnail.sprite = _defaultBG;
                    }

                    SetContentText("스토리 스테이지");
                }
            }
        }


        protected override void GotoStage() => SceneContext.GotoStory();


        private void SetContentText(string text)
        {
            if (_content)
            {
                _content.SetText(text);
            }
        }
    }
}