# Object Pool Manager

## 개요
다른 클래스에서 Awake 이벤트에서 싱글톤 객체에 접근해도 null 오류를 발생시키지 않는 제네릭 싱글톤 클래스이다.

개발의 편의성과 안정적인 동작을 위해 `FindObjectOfType` 메서드를 사용하였으므로 최적화를 위해서는 추후에 의존성 주입 방식으로 전환을 권장한다.

## 클래스 다이어그램
![ClassDiagram](/Documentation/ClassDiagram.drawio.png)

## 순서도
![FlowChart](/Documentation/FlowChart.drawio.png)
