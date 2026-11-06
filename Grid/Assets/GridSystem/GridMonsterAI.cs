using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridTutorial;

[RequireComponent(typeof(GridOccupant), typeof(GridMover))]
public class GridMonsterAI : MonoBehaviour
{
    [SerializeField, Min(0.5f)] float moveInterval = 3f; // 이동 주기 (초)

    GridOccupant occupant;
    GridMover mover;
    float timer;

    // 상하좌우 4방향을 나타내는 오프셋 배열
    readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,    // (0, 1)
        Vector2Int.down,  // (0, -1)
        Vector2Int.left,  // (-1, 0)
        Vector2Int.right  // (1, 0)
    };

    void Awake()
    {
        // 컴포넌트 캐싱
        occupant = GetComponent<GridOccupant>();
        mover = GetComponent<GridMover>();
    }

    void Start()
    {
        // 처음 시작 시 즉시 이동하지 않도록 타이머 초기화
        timer = moveInterval;
    }

    void Update()
    {
        // 몬스터가 이미 이동 중이거나 보드가 없다면 타이머를 굴리지 않음
        if (mover.IsMoving || !occupant.Board)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TryRandomMove();
            timer = moveInterval; // 타이머 재설정
        }
    }

    void TryRandomMove()
    {
        // 주변 4칸 중 이동 가능한 목적지 후보 리스트 생성
        List<Vector2Int> validDestinations = new List<Vector2Int>();

        foreach (var dir in directions)
        {
            Vector2Int targetCell = occupant.Cell + dir;

            // 1. 보드 범위를 벗어나는지 체크 (보드에 IsValidCell 같은 검증 메서드가 있다면 여기서 1차 필터링 가능)
            // 2. GridPathfinder가 길을 찾을 수 있는 셀인지 임시 검증
            // (GridMover.MoveTo 내부에서 실패하면 false를 리턴하므로 시뮬레이션용 후보군에 추가)
            validDestinations.Add(targetCell);
        }

        // 후보군 리스트를 무작위로 섞음 (무작위성 부여)
        ShuffleList(validDestinations);

        // 섞인 순서대로 이동을 시도하고, 최초로 성공(True)하면 즉시 종료
        foreach (var destination in validDestinations)
        {
            if (mover.MoveTo(destination))
            {
                // 이동 성공 시 루프 탈출
                break;
            }
        }
    }

    // 리스트를 무작위로 섞어주는 셔플 함수
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
}
