BEGIN TRAN

IF EXISTS (SELECT 1 FROM W3SI.IDIOMA WHERE COD_IDIOMA = 1)
BEGIN
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'tituloTreinamento', 'Treinamento', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'subtituloTreinamento', 'Reinvente sua gestão de treinamentos utilizando experiências digitais com acesso a indicadores.', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'mensagemPaginaTreinamento', 'Capacitar seu time de forma atrativa e garantir engajamento tem sido um grande desafio? Mude o processo de aprendizagem dos colaboradores com metodologias de gamificação. <br><br>Uma solução fácil de usar que permite centralizar conteúdos, gerenciar treinamentos e ainda obter indicadores para tomada de decisão. <br><br>Enquanto possibilita uma maior autonomia e visibilidade para os gestores sobre o desenvolvimento de suas equipes, resulta em economia de tempo e recursos financeiros pela disponibilização e geração de conhecimento on-line.', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'acessarSolucao', 'Acessar a solução', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'tituloClave', 'Perfil Comportamental', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'subtituloClave', 'Tenha acesso ao mapeamento de perfil dos seus colaboradores e ganhe agilidade e qualidade na tomada de decisão.', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'mensagemPaginaClave', 'Que tal acelerar o alcance de resultados da sua empresa entendendo o potencial do seu time e onde cada um atua com melhor desempenho? <br><br>Com a solução, é possível conhecer profundamente o perfil comportamental e as principais características dos seus colaboradores, já que oferece dados importantes para identificar talentos e gaps a serem tratados. <br><br>Alguns benefícios são: entender as aspirações de carreira e propósito de vida dos funcionários, mapear perfis alinhados à cultura, estratégias e necessidades da empresa e tomar decisões mais acertadas sobre contratação,  desenvolvimento e sucessão.', 'N')
	
	-- MENSAGENS - PORTUGUÊS (MANTENHA ESTE COMENTÁRIO COMO A ÚLTIMA LINHA)
END
COMMIT
