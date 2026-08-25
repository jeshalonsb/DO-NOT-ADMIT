using System.Collections;
using UnityEngine;

public class VisitorManager : MonoBehaviour
{
    [Header("Visitor")]
    [SerializeField] private GameObject visitorPrefab;

    [Header("Visitor Points")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform inspectionPoint;
    [SerializeField] private Transform admitExitPoint;
    [SerializeField] private Transform denyExitPoint;

    [Header("Timing")]
    [SerializeField] private float timeBetweenVisitors = 2f;

    [Header("ID Card")]
    [SerializeField] private IDCard idCard;

    private Visitor currentVisitor;

    private void Start()
    {
        SpawnVisitor();
    }

    private void SpawnVisitor()
    {
        GameObject visitorObject = Instantiate(
            visitorPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        currentVisitor = visitorObject.GetComponent<Visitor>();

        currentVisitor.Setup(
            inspectionPoint,
            admitExitPoint,
            denyExitPoint,
            this
        );
    }

    public void VisitorReady(Visitor visitor)
    {
        currentVisitor = visitor;

        if ( idCard != null ) 
            idCard.DisplayVisitor(visitor.Data);
    }

    public void AdmitCurrentVisitor()
    {
        if (currentVisitor != null)
            currentVisitor.Admit();
    }

    public void DenyCurrentVisitor()
    {
        if (currentVisitor != null)
            currentVisitor.Deny();
    }

    public void VisitorFinished()
    {
        if (idCard != null)
            idCard.HideCard();

        currentVisitor = null;

        StartCoroutine(SpawnNextVisitor());
    }

    private IEnumerator SpawnNextVisitor()
    {
        yield return new WaitForSeconds(timeBetweenVisitors);

        SpawnVisitor();
    }
}