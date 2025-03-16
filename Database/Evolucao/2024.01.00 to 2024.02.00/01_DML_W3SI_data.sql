-- VERIFICAÇÃO DA VERSÃO ATUAL DO BANCO DE DADOS
SET NOEXEC OFF

IF W3IN.FN_VERSAO_IN() <> '2024.1.0' BEGIN
	PRINT 'Versão atual: ' + W3IN.FN_VERSAO_IN()
	RAISERROR('ERRO: Estas alterações são incompatíveis com a versão atual.', 18, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO

UPDATE W3SI.PARAMETRO SET IND_MASCARADO = 'S' WHERE COD_MODULO = 30 AND NOM_PARAMETRO IN('IntegracaoClave.Chave', 'IntegracaoTreinamento.TokenClientSecret')