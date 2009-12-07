namespace PokerTell.Repository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using log4net;

    using Microsoft.Practices.Composite.Events;

    using global::NHibernate;

    using PokerTell.Infrastructure.Events;
    using PokerTell.Infrastructure.Interfaces.DatabaseSetup;
    using PokerTell.Infrastructure.Interfaces.PokerHand;
    using PokerTell.Infrastructure.Interfaces.Repository;
    using PokerTell.Repository.Interfaces;

    public class Repository : IRepository
    {
        #region Constants and Fields

        static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        readonly IRepositoryParser _parser;

        readonly IConvertedPokerHandDao _pokerHandDao;

        readonly ITransactionManager _transactionManager;

        #endregion

        #region Constructors and Destructors

        public Repository(IConvertedPokerHandDao pokerHandDao, ITransactionManager transactionManager, IRepositoryParser parser)
        {
            _transactionManager = transactionManager;
            _pokerHandDao = pokerHandDao;
            _parser = parser;
        }

        #endregion

        #region Implemented Interfaces

        #region IRepository

        public IRepository InsertHand(IConvertedPokerHand convertedPokerHand)
        {
            _transactionManager.Execute((Action)(() => _pokerHandDao.Insert(convertedPokerHand)));

            return this;
        }

        public IRepository InsertHands(IEnumerable<IConvertedPokerHand> handsToInsert)
        {
            Action<IStatelessSession> insertHands = statelessSession => {
                foreach (IConvertedPokerHand convertedHand in handsToInsert)
                {
                    try
                    {
                        _pokerHandDao.Insert(convertedHand, statelessSession);
                    }
                    catch (Exception excep)
                    {
                        Log.Error(excep);
                    }
                }
            };
            _transactionManager.BatchExecute(insertHands);
          
            return this;
        }

        public IConvertedPokerHand RetrieveConvertedHandWith(ulong gameId, string site)
        {
            return _transactionManager.Execute(() => _pokerHandDao.GetHandWith(gameId, site));
        }

        public IConvertedPokerHand RetrieveConvertedHand(int handId)
        {
            return _transactionManager.Execute(() => _pokerHandDao.Get(handId));
        }

        public IEnumerable<IConvertedPokerHand> RetrieveConvertedHands(IEnumerable<int> handIds)
        {
            foreach (int handId in handIds)
            {
                yield return RetrieveConvertedHand(handId);
            }
        }

        public IEnumerable<IConvertedPokerHand> RetrieveHandsFromFile(string fileName)
        {
            string handHistories = ReadHandHistoriesFrom(fileName);

            return _parser.RetrieveAndConvert(handHistories, fileName);
        }

        #endregion

        #endregion

        #region Methods

        static string ReadHandHistoriesFrom(string fileName)
        {
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                // Use UTF7 encoding to ensure correct representation of Umlauts
                return new StreamReader(fileStream, Encoding.UTF7).ReadToEnd();
            }
        }

        #endregion
    }
}