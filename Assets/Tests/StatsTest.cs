using NUnit.Framework;

public class StatsTests
{
    private Health health;
    private Speed speed;
    private Attack attack;
    private Defense defense;

    // Eseguito prima di ogni test.
    // Ogni caso parte con nuove istanze e valori iniziali conosciuti.
    [SetUp]
    public void SetUp()
    {
        health = new Health(100);
        speed = new Speed(4f);
        attack = new Attack(20);
        defense = new Defense(5);
    }

    // ── HEALTH ───────────────────────────────────────────────

    // Verifica che il costruttore utilizzi il massimo richiesto
    // e inizializzi la salute attuale allo stesso valore.
    [Test]
    public void Health_Constructor_UsesProvidedMaxHealth()
    {
        Health customHealth = new Health(120);

        Assert.That(customHealth.GetMaxHealth(), Is.EqualTo(120));
        Assert.That(customHealth.CurrentHealth, Is.EqualTo(120));
    }

    // Verifica che ricevere 20 danni tolga 20 punti vita.
    [Test]
    public void Health_TakeDamage_ReducesHealthBy20()
    {
        health.TakeDamage(20);

        Assert.That(health.CurrentHealth, Is.EqualTo(80));
    }

    // Verifica che ridurre il massimo adegui anche la salute attuale
    // quando questa supera il nuovo limite.
    [Test]
    public void Health_DecreaseMaxHealth_ClampsCurrentHealth()
    {
        health.DecreaseMaxHealth(10);

        Assert.That(health.GetMaxHealth(), Is.EqualTo(90));
        Assert.That(health.CurrentHealth, Is.EqualTo(90));
    }

    // Verifica che il reset utilizzi il massimo aggiornato.
    [Test]
    public void Health_ResetHealth_RestoresCurrentMaximum()
    {
        // Arrange: massimo 90 e salute attuale 60.
        health.DecreaseMaxHealth(10);
        health.TakeDamage(30);

        // Act: ripristina la salute.
        health.ResetHealth();

        // Assert: la salute torna a 90.
        Assert.That(health.CurrentHealth, Is.EqualTo(90));
    }

    // Verifica che aumentare il massimo lasci invariata
    // la salute attuale.
    [Test]
    public void Health_IncreaseMaxHealth_DoesNotHeal()
    {
        health.IncreaseMaxHealth(10);

        Assert.That(health.GetMaxHealth(), Is.EqualTo(110));
        Assert.That(health.CurrentHealth, Is.EqualTo(100));
    }

    // Verifica che una cura eccessiva si fermi al massimo.
    [Test]
    public void Health_Heal_DoesNotExceedMaximum()
    {
        // Arrange: salute attuale 80.
        health.TakeDamage(20);

        // Act: prova a curare 50 punti.
        health.Heal(50);

        // Assert: la salute si ferma a 100.
        Assert.That(health.CurrentHealth, Is.EqualTo(100));
    }

    // Verifica che la cura restituisca la quantità richiesta
    // quando c'è abbastanza spazio prima del massimo.
    [Test]
    public void Health_Heal_RestoresRequestedAmount()
    {
        // Arrange: salute attuale 60.
        health.TakeDamage(40);

        // Act: cura di 10 punti.
        health.Heal(10);

        // Assert: 60 + 10 = 70.
        Assert.That(health.CurrentHealth, Is.EqualTo(70));
    }

    // ── VALORI INIZIALI ──────────────────────────────────────

    // Con moltiplicatore iniziale 1, i valori effettivi
    // coincidono con i valori base.
    [Test]
    public void Stats_Constructors_SetInitialValues()
    {
        Assert.That(speed.CurrentSpeed, Is.EqualTo(4f));
        Assert.That(attack.CurrentDamage, Is.EqualTo(20));
        Assert.That(defense.CurrentDefense, Is.EqualTo(5));
    }

    // ── SPEED ────────────────────────────────────────────────

    // Verifica l'aumento della velocità effettiva,
    // mantenendo invariato il valore base.
    [Test]
    public void Speed_MultiplierChangesCurrentSpeed()
    {
        speed.SetMultiplier(1.5f);

        // 4 × 1,5 = 6.
        Assert.That(speed.CurrentSpeed, Is.EqualTo(6f));
        Assert.That(speed.BaseSpeed, Is.EqualTo(4f));
    }

    // Verifica che il reset rimuova un rallentamento.
    [Test]
    public void Speed_ResetMultiplierRestoresBaseSpeed()
    {
        // Arrange: velocità dimezzata.
        speed.SetMultiplier(0.5f);

        // Act: moltiplicatore riportato a 1.
        speed.ResetMultiplier();

        Assert.That(speed.CurrentSpeed, Is.EqualTo(4f));
    }

    // ── ATTACK ───────────────────────────────────────────────

    // Verifica l'aumento del danno effettivo,
    // mantenendo invariato il danno base.
    [Test]
    public void Attack_MultiplierChangesCurrentDamage()
    {
        attack.SetMultiplier(1.5f);

        // 20 × 1,5 = 30.
        Assert.That(attack.CurrentDamage, Is.EqualTo(30));
        Assert.That(attack.BaseDamage, Is.EqualTo(20));
    }

    // Verifica che il reset rimuova il bonus al danno.
    [Test]
    public void Attack_ResetMultiplierRestoresBaseDamage()
    {
        attack.SetMultiplier(2f);

        attack.ResetMultiplier();

        Assert.That(attack.CurrentDamage, Is.EqualTo(20));
    }

    // ── DEFENSE ──────────────────────────────────────────────

    // Verifica l'aumento della difesa effettiva,
    // mantenendo invariato il valore base.
    [Test]
    public void Defense_MultiplierChangesCurrentDefense()
    {
        defense.SetMultiplier(2f);

        // 5 × 2 = 10.
        Assert.That(defense.CurrentDefense, Is.EqualTo(10));
        Assert.That(defense.BaseDefense, Is.EqualTo(5));
    }

    // Verifica che il reset rimuova il bonus alla difesa.
    [Test]
    public void Defense_ResetMultiplierRestoresBaseDefense()
    {
        defense.SetMultiplier(2f);

        defense.ResetMultiplier();

        Assert.That(defense.CurrentDefense, Is.EqualTo(5));
    }

    // Ogni TestCase esegue il test con una coppia diversa:
    // danno in ingresso e danno finale atteso.
    // La difesa parte sempre da 5.
    [TestCase(20, 15)] // 20 - 5 = 15.
    [TestCase(5, 0)]   // Tutto il danno viene assorbito.
    [TestCase(3, 0)]   // La difesa supera il danno.
    [TestCase(0, 0)]   // Nessun danno.
    [TestCase(-10, 0)] // Il danno negativo viene limitato a zero.
    public void Defense_ReducesDamageWithoutGoingBelowZero(
        int incomingDamage,
        int expectedDamage)
    {
        int damage = defense.ReduceDamage(incomingDamage);

        Assert.That(damage, Is.EqualTo(expectedDamage));
    }

    // ── VALIDAZIONE DELLE STATISTICHE ─────────────────────────

    // Verifica che i valori base negativi siano limitati a zero.
    [Test]
    public void Stats_NegativeBaseValues_AreClampedToZero()
    {
        speed.SetBaseSpeed(-4f);
        attack.SetBaseDamage(-20);
        defense.SetBaseDefense(-5);

        Assert.That(speed.CurrentSpeed, Is.Zero);
        Assert.That(attack.CurrentDamage, Is.Zero);
        Assert.That(defense.CurrentDefense, Is.Zero);
    }

    // Verifica che i moltiplicatori negativi diventino zero,
    // portando a zero anche le statistiche effettive.
    [Test]
    public void Stats_NegativeMultipliers_AreClampedToZero()
    {
        speed.SetMultiplier(-1f);
        attack.SetMultiplier(-1f);
        defense.SetMultiplier(-1f);

        Assert.That(speed.CurrentSpeed, Is.Zero);
        Assert.That(attack.CurrentDamage, Is.Zero);
        Assert.That(defense.CurrentDefense, Is.Zero);
    }

    // ── ATTACK + DEFENSE + HEALTH ─────────────────────────────

    // Verifica l'intero calcolo di un colpo:
    // attacco 20, difesa 5, danno finale 15.
    [Test]
    public void AttackAgainstDefense_AppliesRemainingDamageToHealth()
    {
        int damage = defense.ReduceDamage(attack.CurrentDamage);

        health.TakeDamage(damage);

        // 100 punti vita - 15 danni = 85.
        Assert.That(health.CurrentHealth, Is.EqualTo(85));
    }
}