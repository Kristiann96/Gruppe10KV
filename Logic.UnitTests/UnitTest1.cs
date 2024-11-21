using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;
using Models.Exceptions;
using Interface;
using Logic;

[TestClass]
public class InnmeldingLogicTests
{
    private Mock<ITransaksjonsRepository> _transaksjonsRepositoryMock;
    private Mock<IGeometriRepository> _geometriRepositoryMock;
    private InnmeldingLogic _logic;

    [TestInitialize]
    public void Initialize()
    {
        _transaksjonsRepositoryMock = new Mock<ITransaksjonsRepository>();
        _geometriRepositoryMock = new Mock<IGeometriRepository>();
        _logic = new InnmeldingLogic(_transaksjonsRepositoryMock.Object, _geometriRepositoryMock.Object);
    }

    
}
