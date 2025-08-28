using Match3;
using Match3.Randomizers;
using NUnit.Framework;
using UnityEngine;

public class BoardTests
{
    private readonly ICellsRandomizer cellsRandomizer = new SystemRandomCellsRandomizer();

    [Test]
    public void FiftyGeneratedBoards_NoInitialMatches()
    {
        const int boardsCount = 50;
        const int width = 50;
        const int height = 50;

        for (int i = 0; i < boardsCount; i++)
        {
            Match3Board board = new(width, height, cellsRandomizer);

            Assert.IsFalse(board.HasPossibleMatches(), "There should be no matches on the generated board.");

            Debug.Log("Generated Board Without Matches:");
            Debug.Log(board);
        }
    }

    [Test]
    public void Board50x50_1000MatchesArePossible()
    {
        const int width = 50;
        const int height = 50;
        const int targetMatches = 1000;

        TestHasMatches(width, height, targetMatches);
    }

    [Test]
    public void Board10x10_1000MatchesArePossible()
    {
        const int width = 10;
        const int height = 10;
        const int targetMatches = 1000;

        TestHasMatches(width, height, targetMatches);
    }

    private void TestHasMatches(int width, int height, int targetMatches)
    {
        Match3Board board = new(width, height, cellsRandomizer);

        int matchesFound = 0;
        int movesMade = 0;

        while (matchesFound < targetMatches)
        {
            int randomX = Random.Range(0, width);
            int randomY = Random.Range(0, height);

            Direction randomDirection = (Direction)Random.Range(0, 4);

            try
            {
                bool matchOccurred = board.MoveCell(randomX, randomY, randomDirection, out _) == MoveResult.Match;
                if (matchOccurred)
                {
                    matchesFound++;
                }
            }
            catch
            {
                Debug.Log($"{randomX}, {randomY}, {randomDirection}, matches: {matchesFound}");
                throw;
            }

            movesMade++;
        }

        Assert.AreEqual(targetMatches, matchesFound);
        Debug.Log(movesMade);
    }
}
