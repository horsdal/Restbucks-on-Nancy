namespace RestBucks.Orders.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Infrastructure;
  using Infrastructure.Domain.BaseClass;

  public class Payment : EntityBase
  {
    public virtual string CreditCardNumber { get; set; }
    public virtual string CardOwner { get; set; }
  }

  public class Order : EntityBase, IValidable
  {
    private ISet<OrderItem> items;

    public Order()
    {
      items = new HashSet<OrderItem>();
      Status = OrderStatus.Unpaid;
    }

    public virtual DateTime Date { get; set; }
    public virtual OrderStatus Status { get; set; }
    public virtual Location Location { get; set; }
    public virtual string CancelReason { get; private set; }

    public virtual IEnumerable<OrderItem> Items
    {
      get { return items; }
    }

    public virtual decimal Total
    {
      get { return Items.Sum(i => i.Quantity*i.UnitPrice); }
    }

    public virtual IEnumerable<string> GetErrorMessages()
    {
      if (Items.Count() == 0) yield return "The order must include at least one item.";

      var itemsErrors = Items.SelectMany(
        (i, index) =>
        i.GetErrorMessages().Select(m => string.Format("Item {0}: {1}", index, m)));

      foreach (var itemsError in itemsErrors)
      {
        yield return itemsError;
      }
    }

    public virtual void AddItem(OrderItem orderItem)
    {
      if (Status != OrderStatus.Unpaid)
      {
        throw new InvalidOrderOperationException(string.Format("Can't add another item to the order because it is {0}.",
                                                               Status.ToString().ToLower()));
      }
      orderItem.Order = this;
      items.Add(orderItem);
    }

    public virtual Payment Payment { get; private set; }

    public virtual void Cancel(string cancelReason)
    {
      if (Status != OrderStatus.Unpaid)
      {
        throw new InvalidOrderOperationException(string.Format("The order can not be canceled because it is {0}.",
                                                               Status.ToString().ToLower()));
      }
      CancelReason = cancelReason;
      Status = OrderStatus.Canceled;
      Version++;
    }

    public virtual void Pay(string cardNumber, string cardOwner)
    {
      if (Status != OrderStatus.Unpaid)
      {
        throw new InvalidOrderOperationException(string.Format("The order can not be paid because it is {0}.",
                                                               Status.ToString().ToLower()));
      }
      Status = OrderStatus.Paid;
      Payment = new Payment {CardOwner = cardOwner, CreditCardNumber = cardNumber};
      Version++;
    }

    public virtual void Finish()
    {
      if (Status != OrderStatus.Paid)
      {
        throw new InvalidOrderOperationException(string.Format("The order should not be finished because it is {0}.",
                                                               Status.ToString().ToLower()));
      }
      Status = OrderStatus.Ready;
      Version++;
    }
  }
}