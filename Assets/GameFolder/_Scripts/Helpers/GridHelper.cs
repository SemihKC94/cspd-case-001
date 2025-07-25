using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Random = System.Random;

namespace SKC.Helper
{
    public static class GridHelper
    {
        public static Tuple<int, int> CalculateGridDimensionsForFixedSize(
            int totalCards, RectTransform containerRect,
            float fixedCardWidth, float fixedCardHeight, float cardSpacing,
            float horizontalPaddingTotal, float verticalPaddingTotal)
        {
            float availableWidth = containerRect.rect.width - horizontalPaddingTotal;
            float availableHeight = containerRect.rect.height - verticalPaddingTotal;
            
            int maxPossibleColumns = Mathf.FloorToInt((availableWidth + cardSpacing) / (fixedCardWidth + cardSpacing));
            if (maxPossibleColumns <= 0) maxPossibleColumns = 1; 
            int maxPossibleRows = Mathf.FloorToInt((availableHeight + cardSpacing) / (fixedCardHeight + cardSpacing));
            if (maxPossibleRows <= 0) maxPossibleRows = 1; 
            
            int maxPossibleCells = maxPossibleColumns * maxPossibleRows;
            
            int bestRows = 1;
            int bestColumns = totalCards; 

            int calculatedColumns = maxPossibleColumns;
            while (calculatedColumns > 0)
            {
                if (totalCards % calculatedColumns == 0) 
                {
                    int calculatedRows = totalCards / calculatedColumns;
                    if (calculatedRows <= maxPossibleRows)
                    {
                        bestColumns = calculatedColumns;
                        bestRows = calculatedRows;
                        break;
                    }
                }
                calculatedColumns--;
            }
            
            Debug.Log(bestRows +  "   A   " + bestColumns);
            if (bestColumns == 0)
            {
                bestColumns = maxPossibleColumns > 0 ? maxPossibleColumns : 1;
                bestRows = Mathf.CeilToInt((float)totalCards / bestColumns);
            }
            // if(totalCards >= 18) bestColumns = 10;
            return new Tuple<int, int>(bestRows, bestColumns);
        }

        public static void SetGridLayoutWithFixedSize(
            float fixedCardWidth, float fixedCardHeight, float cardSpacing,
            int calculatedRows, int calculatedColumns,
            GridLayoutGroup gridLayoutGroup, RectTransform cardGridParentRect)
        {
            int rows = calculatedRows;
            int columns = calculatedColumns;

            gridLayoutGroup.cellSize = new Vector2(fixedCardWidth, fixedCardHeight);
            gridLayoutGroup.spacing = new Vector2(cardSpacing, cardSpacing);
            gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;
        }
    }
}