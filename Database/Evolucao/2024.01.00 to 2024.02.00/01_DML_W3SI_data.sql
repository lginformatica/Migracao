-- VERIFICA��O DA VERS�O ATUAL DO BANCO DE DADOS
SET NOEXEC OFF

IF W3IN.FN_VERSAO_IN() <> '2024.1.0' BEGIN
	PRINT 'Vers�o atual: ' + W3IN.FN_VERSAO_IN()
	RAISERROR('ERRO: Estas altera��es s�o incompat�veis com a vers�o atual.', 18, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO

UPDATE W3SI.PARAMETRO SET IND_MASCARADO = 'S' WHERE COD_MODULO = 30 AND NOM_PARAMETRO IN('IntegracaoClave.Chave', 'IntegracaoTreinamento.TokenClientSecret')