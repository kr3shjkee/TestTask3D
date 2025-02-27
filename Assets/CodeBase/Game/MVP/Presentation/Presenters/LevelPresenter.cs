﻿using System.Linq;
using Core.MVP.Presenters;
using Game.Data.Enums;
using Game.Elements;
using Game.MVP.Presentation.Services;
using Game.MVP.Presentation.Views;

namespace Game.MVP.Presentation.Presenters
{
    public class LevelPresenter : IPresenter
    {
        private readonly LevelView _view;
        private readonly LevelService _levelService;

        public LevelPresenter(LevelView view, LevelService levelService)
        {
            _view = view;
            _levelService = levelService;

            _levelService.PrepareLevel += PrepareLevel;
            _levelService.BuildItem += BuildPart;
        }
        
        public void Enable()
        {
            
        }

        public void Disable()
        {
            _levelService.PrepareLevel -= PrepareLevel;
            _levelService.BuildItem -= BuildPart;
        }

        private void PrepareLevel()
        {
            foreach (MagazineElement magazine in _view.MagazineElements)
            {
                magazine.Init();
            }
        }

        private void BuildPart(int id)
        {
            foreach (MagazineElement magazine in _view.MagazineElements)
            {
                magazine.BuildPart(id);
            }
        }
        
    }
}