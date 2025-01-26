using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PolySpatial.Samples
{
    public class LevelBubbleManager : MonoBehaviour
    {
        [SerializeField]
        Transform m_BubbleRoot;

        [SerializeField]
        BubbleLayoutManager m_BubbleLayoutManager;

        [SerializeField]
        LevelData m_LevelData;

        [SerializeField]
        float m_RotateSpeed = 300.0f;

        [SerializeField]
        TMP_Text m_LevelTitle;

        [SerializeField]
        TMP_Text m_LevelDescription;

        List<BubbleSize> m_BubbleSizes = new List<BubbleSize>();
        List<GameObject> m_BubbleObjects;
        List<BubbleCircleNode> m_BubbleCircleNodes;
        float m_StartTime;
        float m_RotationLength;
        Vector3 m_TargetRotation;
        Vector3 m_PreviousRotation;
        const float k_StartingOffset = 180.0f;
        static int s_CurrentSelectedIndex = 0;

        void Start()
        {
            m_BubbleSizes.Clear();
            m_BubbleSizes.AddRange(GetComponentsInChildren<BubbleSize>());
            m_BubbleSizes = new List<BubbleSize>();
            m_BubbleObjects = new List<GameObject>();

            foreach (Transform bubbles in m_BubbleRoot)
            {
                m_BubbleSizes.Add(bubbles.GetComponent<BubbleSize>());
                m_BubbleObjects.Add(bubbles.gameObject);
            }

            UpdateLevelInfo();
            MakeBubbleCircle();
            SetBubbleScale();

            // setup rotation for previous selection
            var rotationValue = 360.0f / m_BubbleObjects.Count;
            m_TargetRotation = new Vector3(0, k_StartingOffset - (s_CurrentSelectedIndex * rotationValue), 0);
        }

        void Update()
        {
#if UNITY_EDITOR
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                ArrowButtonPressed(true);
                m_StartTime = Time.time;
                m_RotationLength = m_BubbleLayoutManager.BubbleSpacing;
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                ArrowButtonPressed(false);
                m_StartTime = Time.time;
                m_RotationLength = m_BubbleLayoutManager.BubbleSpacing;
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                LoadSelectedLevel();
            }
#endif

            var distanceCovered = (Time.time - m_StartTime) * m_RotateSpeed;
            var fractionOfRotation = distanceCovered / m_RotationLength;

            if (distanceCovered > 0)
            {
                m_BubbleRoot.transform.eulerAngles = Vector3.Lerp(m_PreviousRotation, m_TargetRotation, fractionOfRotation);
            }
        }

        public void ArrowButtonPressed(bool left)
        {
            m_StartTime = Time.time;
            m_RotationLength = m_BubbleLayoutManager.BubbleSpacing;

            var direction = left ? 1 : -1;
            m_PreviousRotation = m_TargetRotation;
            m_TargetRotation += new Vector3(0, direction * m_BubbleLayoutManager.BubbleSpacing, 0);

            Debug.Log($"Before updating index: CurrentIndex = {s_CurrentSelectedIndex}, TargetRotation = {m_TargetRotation}, PreviousRotation = {m_PreviousRotation}");


            // cycle index around 0 depending on which button was pressed
            if (s_CurrentSelectedIndex == m_BubbleSizes.Count - 1 && !left)
            {
                Debug.Log($"Transitioning from 6 -> 0");

                s_CurrentSelectedIndex = 0;
            }
            else
            {
                Debug.Log($"Transitioning from 0 -> 6");

                s_CurrentSelectedIndex -= direction;
                if (s_CurrentSelectedIndex < 0)
                {
                    s_CurrentSelectedIndex = m_BubbleSizes.Count - 1;
                }

                if (s_CurrentSelectedIndex >= m_BubbleSizes.Count)
                {
                    s_CurrentSelectedIndex = 0;
                }
            }

            Debug.Log($"After updating index: CurrentIndex = {s_CurrentSelectedIndex}, TargetRotation = {m_TargetRotation}, PreviousRotation = {m_PreviousRotation}");

            UpdateLevelInfo();
            SetBubbleScale();
        }

        void SetBubbleScale()
        {
            // Set scale for bubbles

            Debug.Log($"Setting scales: CurrentSelectedIndex = {s_CurrentSelectedIndex}");
            for (int i = 0; i < m_BubbleSizes.Count; i++)
            {
                Debug.Log($"Bubble {i} current scale = {m_BubbleSizes[i].GetScale()}");
            }
            var currentSelection = m_BubbleSizes[s_CurrentSelectedIndex];
            var nextIndex = (s_CurrentSelectedIndex + 1) % m_BubbleSizes.Count;
            var previousIndex = (s_CurrentSelectedIndex - 1 + m_BubbleSizes.Count) % m_BubbleSizes.Count;

            Debug.Log($"Setting scales for: CurrentBubble = {s_CurrentSelectedIndex}, NextBubble = {nextIndex}, PreviousBubble = {previousIndex}");

            // Large scale for the current selection
            currentSelection.SetScale(BubbleSize.BubbleSizeEnum.Large);

            // Medium scale for the next and previous bubbles
            m_BubbleSizes[nextIndex].SetScale(BubbleSize.BubbleSizeEnum.Medium);
            m_BubbleSizes[previousIndex].SetScale(BubbleSize.BubbleSizeEnum.Medium);

            // Small scale for all other bubbles
            for (int i = 0; i < m_BubbleSizes.Count; i++)
            {
                if (i != s_CurrentSelectedIndex && i != nextIndex && i != previousIndex)
                {
                    m_BubbleSizes[i].SetScale(BubbleSize.BubbleSizeEnum.Small);
                }
            }
            for (int i = 0; i < m_BubbleSizes.Count; i++)
            {
                Debug.Log($"Bubble {i} new scale = {m_BubbleSizes[i].GetScale()}");
            }
        }

        public void LoadSelectedLevel()
        {
            m_BubbleObjects[s_CurrentSelectedIndex].GetComponent<LoadLevelButton>().Press();
        }

        void MakeBubbleCircle()
        {
            m_BubbleCircleNodes = new List<BubbleCircleNode>();

            for (int i = 0; i < m_BubbleObjects.Count; i++)
            {
                var nextIndex = (i + 1) % m_BubbleObjects.Count;
                var previousIndex = (i - 1 + m_BubbleObjects.Count) % m_BubbleObjects.Count;

                var nextBubble = m_BubbleObjects[nextIndex];
                var previousBubble = m_BubbleObjects[previousIndex];
                var newBubbleNode = new BubbleCircleNode(m_BubbleObjects[i], nextBubble, previousBubble);
                m_BubbleCircleNodes.Add(newBubbleNode);
            }
        }

        void UpdateLevelInfo()
        {
            var levelType = m_BubbleObjects[s_CurrentSelectedIndex].GetComponent<LoadLevelButton>().LevelType;
            m_LevelTitle.text = m_LevelData.GetLevelTitle(levelType);
            m_LevelDescription.text = m_LevelData.GetLevelDescription(levelType);
        }
    }

    struct BubbleCircleNode
    {
        public GameObject Bubble;
        public GameObject NextBubble;
        public GameObject PreviousBubble;

        public BubbleCircleNode(GameObject bubble, GameObject nextBubble, GameObject previousBubble)
        {
            Bubble = bubble;
            NextBubble = nextBubble;
            PreviousBubble = previousBubble;
        }
    }
}
