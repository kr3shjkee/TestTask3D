﻿using System.Linq;
using Core.MVP.Presenters;
using Game.Data.Dto;
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
            _levelService.PrepareStonesProgress += PrepareStonesProgress;
            Subscribe();
        }
        
        public void Enable()
        {
            
        }

        public void Disable()
        {
            _levelService.PrepareLevel -= PrepareLevel;
            _levelService.BuildItem -= BuildPart;
            _levelService.PrepareStonesProgress -= PrepareStonesProgress;
            
            Unsubscribe();
        }

        private void Subscribe()
        {
            foreach (MagazineElement magazine in _view.MagazineElements)
            {
                magazine.InvokeShowMagazineProgress += ShowMagazineProgress;
                magazine.InvokeStonesProgress += UpdateStonesProgress;
            }
        }

        private void Unsubscribe()
        {
            foreach (MagazineElement magazine in _view.MagazineElements)
            {
                magazine.InvokeShowMagazineProgress -= ShowMagazineProgress;
                magazine.InvokeStonesProgress -= UpdateStonesProgress;
            }
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

        private void ShowMagazineProgress(MagazineProgressDto dto, bool isAnimation)
        {
            _levelService.InvokeShowProgressBar(dto,isAnimation);
            if (_view.MagazineElements.All(item => item.Status == BuildingStatus.Builded))
            {
                _levelService.InvokeWinGame();
            }
        }

        private void UpdateStonesProgress(StonesProgressDto dto)
        {
            _levelService.InvokeUpdateStonesProgress(dto);
        }

        private void PrepareStonesProgress(MagazineType type)
        {
            MagazineElement magazineElement = _view.MagazineElements.FirstOrDefault(item => item.Type == type);
            
            if(magazineElement != null)
                magazineElement.UpdateStonesProgress();
        }
    }
}