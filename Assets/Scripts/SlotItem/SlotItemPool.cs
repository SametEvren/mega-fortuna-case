﻿using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Utility;

namespace SlotItem
{
    public class SlotItemPool : MonoBehaviour, IObjectPoolManager<SlotItemController>
    {
        private ObjectPool<SlotItemController> _slotItemPool;
        [SerializeField] private SlotItemController slotItemPrefab;
        private const int SlotItemCapacity = 2000;

        private void Awake()
        {
            SetPool();
            ServiceLocator.Add<IObjectPoolManager<SlotItemController>>(this);
        }

        #region SetPool

        private void SetPool()
        {
            _slotItemPool = CreatePool(slotItemPrefab, SlotItemCapacity);
        }
    
        private ObjectPool<SlotItemController> CreatePool(SlotItemController prefab, int capacity)
        {
            return new ObjectPool<SlotItemController>(() => Instantiate(prefab), ActionOnGet, OnPutBackInPool, defaultCapacity: capacity);
        }

        private void ActionOnGet(SlotItemController obj)
        {
            obj.gameObject.SetActive(true);
        }

        private void OnPutBackInPool(SlotItemController obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(null);
        }

        #endregion

        public SlotItemController GetItemFromPool()
        {
            var slotItem = _slotItemPool.Get();
            slotItem.ResetSlotItem();
            return slotItem;    
        }

        public void ReleaseItemToPool(SlotItemController item)
        {
            OnPutBackInPool(item);
            StartCoroutine(ReleaseAfterTime(1.1f));
            IEnumerator ReleaseAfterTime(float time)
            {
                yield return new WaitForSeconds(time);
                _slotItemPool.Release(item);
            }
        }
        
    }
}